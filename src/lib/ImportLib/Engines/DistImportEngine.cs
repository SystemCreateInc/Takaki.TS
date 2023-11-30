
using CsvHelper;
using CsvHelper.Configuration;
using DbLib;
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
    public class DistImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<TargetImportFile> _targetImportFiles { get; private set; } = new List<TargetImportFile>();

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
                var importDatas = new List<DistFileLine>();

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

        private async Task<int> InsertData(IEnumerable<DistFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                await Task.Yield();

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

        private async Task<IEnumerable<DistFileLine>> ReadFileAsync(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<DistFileLine>();

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
