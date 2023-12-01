using CsvHelper;
using CsvHelper.Configuration;
using DbLib;
using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using Microsoft.IdentityModel.Tokens;
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

        public List<TargetImportFile> _targetImportFiles { get; private set; } = new List<TargetImportFile>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<StowageImportEngine>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

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

                _targetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
                {
                    Syslog.Debug($"found file: {x}");
                    var fi = new FileInfo(x);
                    return new TargetImportFile
                    {
                        Selected = true,
                        ImportFileSize = fi.Length,
                        ImportFileLastWriteDateTime = fi.LastWriteTime,
                        ImportFilePath = x,
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
                var importDatas = new List<StowageFileLine>();
                repo.DeleteExpiredStowageData();

                foreach (var targetFile in _targetImportFiles)
                {
                    var beforeCount = importDatas.Count;
                    importDatas.AddRange(ReadFile(token, targetFile.ImportFilePath));
                    importResults.Add(new ImportResult(true, (long)targetFile.ImportFileSize!, importDatas.Count - beforeCount));
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
            var importDatas = new List<StowageFileLine>();

            foreach (var targetFile in _targetImportFiles)
            {
                importDatas.AddRange(ReadFile(token, targetFile.ImportFilePath));
            }

            var distKeyGroup = importDatas.GroupBy(x => new { x.DtDelivery, x.CdShukkaBatch })
                .Select(x => new SameDistInfo
                {
                    DtDelivery = x.Key.DtDelivery,
                    ShukkaBatch = x.Key.CdShukkaBatch,
                });

            using (var repo = new ImportRepository())
            {
                return repo.GetDeleteSameStowageDatas(distKeyGroup);
            }
        }

        private int InsertData(IEnumerable<StowageFileLine> datas, ImportRepository repo, CancellationToken token)
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
                    MissingFieldFound = null,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        token.ThrowIfCancellationRequested();

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
