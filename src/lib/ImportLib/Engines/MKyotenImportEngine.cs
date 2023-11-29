
using CsvHelper;
using CsvHelper.Configuration;
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
    public class MKyotenImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<TargetImportFile> targetImportFiles { get; private set; } = new List<TargetImportFile>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MKyotenImportEngine>();

        public MKyotenImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
        }

        public void UpdateImportFileInfo()
        {
            try
            {
                var dir = Path.GetDirectoryName(_interfaceFile.FileName);
                var fileName = Path.GetFileName(_interfaceFile.FileName);
                if (dir.IsNullOrEmpty() || fileName.IsNullOrEmpty())
                {
                    return;
                }

                targetImportFiles = Directory.EnumerateFiles(dir!, fileName).Select(x =>
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
                targetImportFiles.Clear();
            }
        }

        public async Task<List<ImportResult>> ImportAsync(CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                var importResults = new List<ImportResult>();
                var importDatas = new List<KyotenFileLine>();

                foreach (var targetFile in targetImportFiles)
                {
                    repo.DeleteExpiredKyotenData();
                    importDatas.AddRange(await ReadFileAsync(token, targetFile.ImportFilePath));

                    var beforeCount = importDatas.Count;
                    importResults.Add(new ImportResult(true, (long)targetFile.ImportFileSize!, importDatas.Count - beforeCount));
                }

                await InsertData(importDatas, repo, token);

                repo.Commit();

                return importResults;
            }
        }

        private async Task<int> InsertData(IEnumerable<KyotenFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                await Task.Yield();

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
            }

            return importedCount;
        }

        private async Task<IEnumerable<KyotenFileLine>> ReadFileAsync(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<KyotenFileLine>();

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
                }
            }
            catch (Exception ex)
            {
                var message = $"CSVファイルの読み込み中にエラーが発生しました\n{ex.Message}";
                throw new Exception(message);
            }

            return datas;
        }

        // 変更した受信パスでDB側Path更新
        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = _interfaceFile.FileName };
        }
    }
}
