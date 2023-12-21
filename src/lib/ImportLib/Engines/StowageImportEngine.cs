using CsvHelper;
using CsvHelper.Configuration;
using DbLib;
using DbLib.Defs;
using ImportLib.CSVModels;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Services.Dialogs;
using System.Globalization;
using System.IO;
using System.Text;

namespace ImportLib.Engines
{
    public class StowageImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> TargetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<StowageImportEngine>();

        public StowageImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
            ImportFilePath = _interfaceFile.FileName;
        }

        public void UpdateImportFileInfo()
        {
            try
            {
                var dir = Path.GetDirectoryName(ImportFilePath);
                var fileName = Path.GetFileName(ImportFilePath);
                if (dir.IsNullOrEmpty() || fileName.IsNullOrEmpty())
                {
                    return;
                }

                TargetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
                {
                    Syslog.Debug($"found file: {x}");
                    var fi = new FileInfo(x);
                    return new ImportFileInfo
                    {
                        Selected = fi.Length > 0,
                        Name = DataName,
                        FileSize = fi.Length,
                        LastWriteTime = fi.LastWriteTime,
                        FilePath = x,
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Warn($"UpdateImportFileInfo: {ex}");
                TargetImportFiles.Clear();
            }
        }

        public IEnumerable<ImportResult> Import(DataImportController controller, CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                var sameDistInfos = GetSameDistData(repo, token);
                TestImportData(sameDistInfos, controller);

                var importResults = new List<ImportResult>();
                repo.DeleteExpiredStowageData();

                foreach (var targetFile in TargetImportFiles)
                {
                    var importDatas = ReadFile(token, targetFile.FilePath!);
                    var importedCount = InsertData(controller, importDatas, repo, token);
                    importResults.Add(new ImportResult(true, targetFile.FilePath!, (long)targetFile.FileSize!, importedCount));
                }

                repo.Commit();
                return importResults;
            }
        }

        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }

        private void TestImportData(IEnumerable<SameDistInfo> sameDistInfos, DataImportController controller)
        {
            if (sameDistInfos.Any(x => x.IsWork))
            {
                throw new Exception("同じ納品日・出荷バッチコードのデータで作業済みがある為中断します。");
            }

            if (sameDistInfos.Any())
            {
                var message = "同じ納品日・出荷バッチコードのデータが登録されています。\n入れ替えますか？";
                if (controller.Confirm(message, DataName) != ButtonResult.OK)
                {
                    throw new OperationCanceledException(message);
                }
            }
        }

        // 同一Dist情報取得
        private IEnumerable<SameDistInfo> GetSameDistData(ImportRepository repo, CancellationToken token)
        {
            var importDatas = new List<StowageFileLine>();

            foreach (var targetFile in TargetImportFiles)
            {
                importDatas.AddRange(ReadFile(token, targetFile.FilePath!));
            }

            var distKeyGroup = importDatas
                .GroupBy(x => new { x.DtDelivery, x.CdShukkaBatch })
                .Select(x => new SameDistInfo
                {
                    DtDelivery = x.Key.DtDelivery,
                    ShukkaBatch = x.Key.CdShukkaBatch,
                });

            return repo.GetDeleteSameStowageDatas(distKeyGroup);
        }

        private int InsertData(DataImportController controller, IEnumerable<StowageFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

                repo.Insert(new DbLib.DbEntities.TBSTOWAGEEntity
                {
                    DTDELIVERY = line.DtDelivery,
                    CDSHUKKABATCH = line.CdShukkaBatch,
                    CDKYOTEN = line.CdKyoten,
                    CDHAISHOBIN = line.CdHaishoBin,
                    CDCOURSE = line.CdCourse,
                    CDROUTE = line.CdRoute,
                    CDTOKUISAKI = line.CdTokuisaki,
                    CDHENKOSHA = string.Empty,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    FGSSTATUS = (short)Status.Ready,
                    STBOXTYPE = line.StBoxtype,
                    NUOBOXCNT = line.NuOboxcnt,
                    NURBOXCNT = 0,
                    NMHENKOSHA = line.NmHenkosha,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;

                if ((importedCount % 33) == 0)
                {
                    controller.NotifyProgress("取込中", importedCount, datas.Count());
                }
            }

            return importedCount;
        }

        private IEnumerable<StowageFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<StowageFileLine>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var line = csv.GetRecord<StowageFileLine>();
                            if (line is not null)
                            {
                                datas.Add(line);
                            }
                            else
                            {
                                _logger.Warn($"Line is null");
                            }
                        }
                        catch (CsvHelper.MissingFieldException)
                        {
                            _logger.Warn($"MissingField Skip Row={csv.Parser.Row} Length={csv.Parser.Record?.Length ?? 0}");
                            continue;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var message = $"CSVファイルの読み込み中にエラーが発生しました\n{ex.Message}";
                throw new Exception(message);
            }

            return datas;
        }
    }
}
