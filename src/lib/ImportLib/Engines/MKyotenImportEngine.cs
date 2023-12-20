
using CsvHelper;
using CsvHelper.Configuration;
using DbLib;
using DbLib.Defs.DbLib.Defs;
using ImportLib.CSVModels;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IO;
using System.Text;

namespace ImportLib.Engines
{
    public class MKyotenImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> TargetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MKyotenImportEngine>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

        public MKyotenImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
            ImportFilePath = interfaceFile.FileName;
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
                var importResults = new List<ImportResult>();
                repo.DeleteKyotenData();

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

        // 変更した受信パスでDB側Path更新
        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }

        public Task<bool> SetSameDist(CancellationToken token)
        {
            // 処理無し
            return Task.Run(() => true);
        }

        private int InsertData(DataImportController controller, IEnumerable<KyotenFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

                repo.Insert(new DbLib.DbEntities.TBMKYOTENEntity
                {
                    CDKYOTEN = line.CdKyoten,
                    DTTEKIYOKAISHI = line.DtTekiyokaishi,
                    DTTEKIYOMUKO = line.DtTekiyomuko,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,
                    NMKYOTEN = line.NmKyoten,
                    CDKYOTENSHUBETSU = line.CdKyotenShubetsu,
                    CDTORIHIKISAKI = line.CdTorihikisaki,
                    CDZAIKOHIKIATEBUMON = line.CdZaikoHikiateBumon,
                    NMKYOTENRYAKUSHO = line.NmKyotenRyakusho,
                    CDKYOTENBUMON = line.CdKyotenBumon,
                    STSESANKANRINIPPAIHIN = line.StSesankanriNippaihin,
                    STSEISANKANRIZAIKOHIN = line.StSeisankanriZaikohin,
                    STSHIKIBETSU = line.StShikibetsu,
                    CDTENPOBRAND = line.CdTenpoBrand,
                    CDBASHO = line.CdBasho,
                    CDKYOTENZOKUSEI = line.CdKyotenZouusei,
                    NUHAITA = line.NuHaita ?? 0,
                    DTRENKEI = line.DtRenkei,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;

                if ((importedCount % 33) == 0)
                {
                    controller.NotifyProgress($"{importedCount}/{datas.Count()}");
                }
            }

            return importedCount;
        }

        private IEnumerable<KyotenFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<KyotenFileLine>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var line = csv.GetRecord<KyotenFileLine>();
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
