
using CsvHelper;
using CsvHelper.Configuration;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using System.Globalization;
using System.IO;
using System.Text;

namespace ImportLib.Engines
{
    public class MKyotenImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; }

        public long? ImportFileSize { get; private set; }

        public DateTime? ImportFileLastWriteDateTime { get; private set; }

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MKyotenImportEngine>();

        public MKyotenImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
            ImportFilePath = _interfaceFile.FileName;
        }

        public void UpdateImportFileInfo()
        {
            try
            {
                var fileInfo = new FileInfo(ImportFilePath);
                ImportFileSize = fileInfo.Length;
                ImportFileLastWriteDateTime = fileInfo.LastWriteTime;
            }
            catch (Exception ex)
            {
                _logger.Warn($"UpdateImportFileInfo: {ex}");
                ImportFileSize = null;
                ImportFileLastWriteDateTime = null;
            }
        }

        public async Task<ImportResult> ImportAsync(CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                repo.DeleteExpiredKyotenData();
                var datas = await ReadFileAsync(token);
                var importCount = await InsertData(datas, repo, token);
                repo.Commit();

                return new ImportResult(true, (long)ImportFileSize!, importCount);
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

        private async Task<IEnumerable<KyotenFileLine>> ReadFileAsync(CancellationToken token)
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

                using (var reader = new StreamReader(ImportFilePath, Encoding.GetEncoding("shift_jis")))
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

        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }
    }
}
