
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
    public class DistImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> _targetImportFiles { get; private set; } = new List<ImportFileInfo>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

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

                _targetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
                {
                    Syslog.Debug($"found file: {x}");
                    var fi = new FileInfo(x);
                    return new ImportFileInfo
                    {
                        Selected = true,
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

        public async Task<List<ImportResult>> ImportAsync(CancellationToken token)
        {
            return await Task.Run(() => Import(token));
        }

        public List<ImportResult> Import(CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                var importResults = new List<ImportResult>();
                var importDatas = new List<DistFileLine>();
                repo.DeleteExpiredDistData();
                repo.DeleteSameDistData(SameDistInfos);

                foreach (var targetFile in _targetImportFiles)
                {
                    if (!targetFile.Selected)
                    {
                        continue;
                    }

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

        public async Task<bool> SetSameDist(CancellationToken token)
        {
            SameDistInfos = await Task.Run(() => GetSameDistDataAsync(token));
            return SameDistInfos.Any();
        }

        // 同一Dist情報取得
        private IEnumerable<SameDistInfo> GetSameDistDataAsync(CancellationToken token)
        {
            var importDatas = new List<DistFileLine>();

            foreach (var targetFile in _targetImportFiles)
            {
                if (!targetFile.Selected)
                {
                    continue;
                }

                importDatas.AddRange(ReadFile(token, targetFile.FilePath!));
            }

            var distKeyGroup = importDatas.GroupBy(x => new { x.DtDelivery, x.CdShukkaBatch })
                .Select(x => new SameDistInfo
                {
                    DtDelivery = x.Key.DtDelivery,
                    ShukkaBatch = x.Key.CdShukkaBatch,
                });

            using (var repo = new ImportRepository())
            {
                return repo.GetDeleteSameDistDatas(distKeyGroup);
            }
        }

        private int InsertData(IEnumerable<DistFileLine> datas, ImportRepository repo, CancellationToken token)
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
