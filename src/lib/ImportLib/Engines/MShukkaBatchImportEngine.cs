
using CsvHelper;
using CsvHelper.Configuration;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Models;
using ImportLib.Repositories;
using LogLib;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using static System.Windows.Forms.AxHost;

namespace ImportLib.Engines
{
    public class MShukkaBatchImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; }

        public long? ImportFileSize { get; private set; }

        public DateTime? ImportFileLastWriteDateTime { get; private set; }

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MShukkaBatchImportEngine>();

        public MShukkaBatchImportEngine(InterfaceFile interfaceFile)
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

        private async Task<int> InsertData(IEnumerable<ShukkaBatchFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                await Task.Yield();

                repo.Insert(new DbLib.DbEntities.TBMSHUKKABATCHEntity
                {
                    CDSHUKKABATCH = line.CdShukkaBatch,
                    DTTEKIYOKAISHI = line.DtTekiyokaishi,
                    DTTEKIYOMUKO = line.DtTekiyomuko,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,
                    NMSHUKKABATCH = line.NmShukkaBatch,
                    CDSHIWAKEKYOTEN = line.CdShiwakeKyoten,
                    TMSHIWAKEKAISHI = line.TmShiwakeKaishi,
                    TMSHIWAKESHURYO = line.TmShiwakeShuryo,
                    CDSHUKKABATCHGROUP = line.CdShukkaBatchGroup,
                    STOSHIWAKEHYOSHUBETSU = line.StOshiwakehyoShubetsu,
                    STCHUSHIWAKEHYOSHUBETSU = line.StChushiwakehyoShubetsu,
                    STKOSHIWAKEHYOSHUBETSU = line.StKoshiwakehyoShubetsu,
                    FGTOKUISAKISHUKEI1 = line.FgTokuisakiShukei1,
                    FGTOKUISAKISHUKEI2 = line.FgTokuisakiShukei2,
                    FGTOKUISAKISHUKEI3 = line.FgTokuisakiShukei3,
                    FGTOKUISAKISHUKEI4 = line.FgTokuisakiShukei4,
                    FGTOKUISAKISHUKEI5 = line.FgTokuisakiShukei5,
                    FGTOKUISAKISHUKEI6 = line.FgTokuisakiShukei6,
                    FGHAISOUBINBETSUSHUKEIUMU = line.FgHaisoubinBetsuShukeiUmu,
                    FGCOURSEBETSUSHUKEIUMU = line.FgCourseBetsuShukeiUmu,
                    FGJUCHUJOKYOHYO = line.FgJuchujokyohyo,
                    FGSANDHIKITORIHYO = line.FgSandHikitoriHyo,
                    STDPSSHUBETSU = line.StDpsShubetsu,
                    NUSHIWAKELEADTIME = line.NuShiwakeLeadTime ?? 0,
                    STLEADTIMESEIGYO = line.StLeadTimeSeigyo,
                    FGYOBI1 = line.FgYobi1,
                    FGYOBI2 = line.FgYobi2,
                    FGYOBI3 = line.FgYobi3,
                    FGYOBI4 = line.FgYobi4,
                    FGYOBI5 = line.FgYobi5,
                    STYOBI1 = line.StYobi1,
                    STYOBI2 = line.StYobi2,
                    STYOBI3 = line.StYobi3,
                    STYOBI4 = line.StYobi4,
                    STYOBI5 = line.StYobi5,
                    NUHAITA = line.NuHaita ?? 0,
                    DTRENKEI = line.DtRenkei,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;
            }

            return importedCount;
        }

        private async Task<IEnumerable<ShukkaBatchFileLine>> ReadFileAsync(CancellationToken token)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<ShukkaBatchFileLine>();

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

                        var line = csv.GetRecord<ShukkaBatchFileLine>();
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
