
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
    public class DistImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> TargetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<DistImportEngine>();

        public DistImportEngine(InterfaceFile interfaceFile)
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
                repo.DeleteExpiredDistData();
                repo.DeleteSameDistData(sameDistInfos);

                foreach (var targetFile in TargetImportFiles)
                {
                    if (!targetFile.Selected)
                    {
                        Syslog.Debug($"Import file skip {targetFile.Name}");
                        continue;
                    }

                    Syslog.Debug($"Import file {targetFile.Name}");
                    Syslog.SLCopy(targetFile.FilePath!);

                    var importDatas = ReadFile(token, targetFile.FilePath!);
                    var insertedCount = InsertData(controller, importDatas, repo, token);
                    importResults.Add(new ImportResult(true, targetFile.FilePath!, (long)targetFile.FileSize!, insertedCount));
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
                throw new Exception("同じ納品日・出荷バッチコードで仕分開始しているデータがある為、中断します。");
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
            var importDatas = new List<DistFileLine>();

            foreach (var targetFile in TargetImportFiles)
            {
                if (!targetFile.Selected)
                {
                    continue;
                }

                importDatas.AddRange(ReadFile(token, targetFile.FilePath!));
            }

            var distKeyGroup = importDatas
                .GroupBy(x => new { x.DtDelivery, x.CdShukkaBatch })
                .Select(x => new SameDistInfo
                {
                    DtDelivery = x.Key.DtDelivery,
                    ShukkaBatch = x.Key.CdShukkaBatch,
                });

            return repo.GetDeleteSameDistDatas(distKeyGroup);
        }

        private int InsertData(DataImportController controller, IEnumerable<DistFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

                repo.Insert(new DbLib.DbEntities.TBDISTEntity
                {
                    DTDELIVERY = line.DtDelivery,
                    CDSHUKKABATCH = line.CdShukkaBatch,
                    CDKYOTEN = line.CdKyoten,
                    CDHAISHOBIN = line.CdHaishoBin,
                    CDJUCHUBIN = line.CdJuchuBin,
                    CDCOURSE = line.CdCourse,
                    CDROUTE = line.CdRoute,
                    CDTOKUISAKI = line.CdTokuisaki,
                    CDHIMBAN = line.CdHimban,
                    CDGTIN13 = line.CdGtin13,
                    CDGTIN14 = line.CdGtin14,
                    STBOXTYPE = line.StBoxtype,
                    NUBOXUNIT = line.NuBoxunit,
                    NUOPS = line.NuOps,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,

                    NULOPS = 0,
                    NUDOPS = 0,
                    NUDRPS = 0,
                    NULRPS = 0,
                    FGMAPSTATUS = 0,
                    FGLSTATUS = 0,
                    FGDSTATUS = 0,

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

        private IEnumerable<DistFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<DistFileLine>();

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
                            var line = csv.GetRecord<DistFileLine>();
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
