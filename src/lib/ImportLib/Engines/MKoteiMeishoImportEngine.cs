
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
    public class MKoteiMeishoImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> _targetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MKoteiMeishoImportEngine>();
        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

        public MKoteiMeishoImportEngine(InterfaceFile interfaceFile)
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

                _targetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
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
                _targetImportFiles.Clear();
            }
        }

        public IEnumerable<ImportResult> Import(CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                var importResults = new List<ImportResult>();
                var importDatas = new List<KoteiMeishoFileLine>();
                repo.DeleteExpiredKoteimeishoData();

                foreach (var targetFile in _targetImportFiles)
                {
                    var beforeCount = importDatas.Count;
                    importDatas.AddRange(ReadFile(token, targetFile.FilePath!));
                    importResults.Add(new ImportResult(true, (long)targetFile.FileSize!, importDatas.Count - beforeCount));
                }

                InsertData(importDatas, repo, token);

                repo.Commit();

                return importResults;
            }
        }
        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }

        public Task<bool> SetSameDist(CancellationToken token)
        {
            // 処理無し
            return Task.Run(() => true);
        }

        private int InsertData(IEnumerable<KoteiMeishoFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

                repo.Insert(new DbLib.DbEntities.TBMKOTEIMEISHOEntity
                {
                    CDMEISHOSHIKIBETSU = line.CdMeishoShikibetsu,
                    CDMEISHO = line.CdMeisho,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,
                    NM = line.Nm,
                    NMYOMI = line.NmYomi,
                    NMRYAKU = line.NmRyaku,
                    NMRYAKUYOMI = line.NmRyakuYomi,
                    CDKYUMEISHOSHIKIBETSU = line.CdKyuMeishoShikibetsu,
                    CDKYUMEISHO = line.CdKyuMeisho,
                    CDEX1 = line.CdEx1,
                    CDEX2 = line.CdEx2,
                    CDEX3 = line.CdEx3,
                    CDEX4 = line.CdEx4,
                    CDEX5 = line.CdEx5,
                    FGEX1 = line.FgEx1,
                    FGEX2 = line.FgEx2,
                    FGEX3 = line.FgEx3,
                    FGEX4 = line.FgEx4,
                    FGEX5 = line.FgEx5,
                    NUHAITA = line.NuHaita ?? 0,
                    DTRENKEI = line.DtRenkei,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;
            }

            return importedCount;
        }

        private IEnumerable<KoteiMeishoFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<KoteiMeishoFileLine>();

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
                            var line = csv.GetRecord<KoteiMeishoFileLine>();
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
