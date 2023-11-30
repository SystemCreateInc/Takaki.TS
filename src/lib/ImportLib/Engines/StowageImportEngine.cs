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
            using (var repo = new ImportRepository())
            {
                var importResults = new List<ImportResult>();
                var importDatas = new List<StowageFileLine>();

                foreach (var targetFile in _targetImportFiles)
                {
                    repo.DeleteExpiredKyotenData();
                    importDatas.AddRange(await ReadFileAsync(token, targetFile.ImportFilePath));

                    var beforeCount = importDatas.Count;
                    importResults.Add(new ImportResult(true, (long)targetFile.ImportFileSize!, importDatas.Count - beforeCount));
                }

                // TODO:同一Dist存在時の確認メッセージ
                //var sameDistData = GetSameDistData(importDatas);

                //if (sameDistData.Any())
                //{
                //    var sameDistStr = string.Join(",\n", sameDistData.Select(x => $"{x.DtDelivery}, {x.CdShukkaBatch}"));

                //}

                await InsertData(importDatas, repo, token);

                repo.Commit();

                return importResults;
            }
        }

        private List<DistFileLine> GetSameDistData(List<DistFileLine> importDatas)
        {
            throw new NotImplementedException();
        }

        private async Task<int> InsertData(IEnumerable<StowageFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                await Task.Yield();

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

        private async Task<IEnumerable<StowageFileLine>> ReadFileAsync(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<StowageFileLine>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    MissingFieldFound = null,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        await Task.Yield();

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
            catch (Exception ex)
            {
                var message = $"CSVファイルの読み込み中にエラーが発生しました\n{ex.Message}";
                throw new Exception(message);
            }

            return datas;
        }

        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }
    }
}
