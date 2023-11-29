
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
    public class MTokuisakiImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<TargetImportFile> targetImportFiles { get; private set; } = new List<TargetImportFile>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MTokuisakiImportEngine>();

        public MTokuisakiImportEngine(InterfaceFile interfaceFile)
        {
            _interfaceFile = interfaceFile;
            ImportFilePath = _interfaceFile.FileName;
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
                var importDatas = new List<TokuisakiFileLine>();

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

        private async Task<int> InsertData(IEnumerable<TokuisakiFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                await Task.Yield();

                repo.Insert(new DbLib.DbEntities.TBMTOKUISAKIEntity
                {
                    CDTOKUISAKI = line.CdTokuisaki,
                    DTTEKIYOKAISHI = line.DtTekiyokaishi,
                    DTTEKIYOMUKO = line.DtTekiyomuko,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,
                    NMTOKUISAKI = line.NmTokuisaki,
                    NMTOKUISAKIYOMI = line.NmTokuisakiYomi,
                    NMJUSHO1 = line.NmJusho1,
                    NMJUSHO2 = line.NmJusho2,
                    NMJUSHOYOMI = line.NmJushoYomi,
                    IFYUBINBANGO = line.IfYubinBango,
                    IFDENWABANGO = line.IfDenwaBango,
                    IFKENSAKUYODENWABANGO = line.IfKensakuyoDenwaBango,
                    IFFAXBANGO = line.IfFaxBango,
                    CDCHIIKI = line.CdChiiki,
                    DTKAITEN = line.DtKaiten,
                    DTHEITEN = line.DtHeiten,
                    DTKEIYAKU = line.DtKeiyaku,
                    DTKAIYAKU = line.DtKaiyaku,
                    STCHANEL = line.StChanel,
                    CDKANKATSUEIGYOBUMON = line.CdKankatsuEigyobumon,
                    CDTANTOGYOMUKYOTEN = line.CdTantoGyomukyoten,
                    STGYOTAI = line.StGyotai,
                    STTOKUISAKISHIKIBETSU = line.StTokuisakiShikibetsu,
                    STTOKUISAKISHUBETSU = line.StTokuisakiShubetsu,
                    NOOYASHOTEN = line.NoOyashoten,
                    NOSOSHIKI1 = line.NoSoshiki1,
                    NOSOSHIKI2 = line.NoSoshiki2,
                    CDKEIYAKUGROUP = line.CdKeiyakuGroup,
                    NOKEIYAKUSEDAI = line.NoKeiyakuSedai,
                    CDSHODANGROUP = line.CdShodanGroup,
                    NOSHODANSEDAI = line.NoShodanSedai,
                    CDJUCHUGROUP = line.CdJuchuGroup,
                    NOJUCHUSEDAI = line.NoJuchuSedai,
                    CDNOHINGROUP = line.CdNohinGroup,
                    NONOHINSEDAI = line.NoNohinSedai,
                    CDSEIKYUGROUP = line.CdSeikyuGroup,
                    NOSEIKYUSEDAI = line.NoSeikyuSedai,
                    CDNYUKINGROUP = line.CdNyukinGroup,
                    NONYUKINSEDAI = line.NoNyukinSedai,
                    STKAZEI = line.StKazei,
                    STSHOHIZEIKEISAN = line.StShohizeiKeisan,
                    STNAIBUURIAGEKOZA = line.StNaibuUriageKoza,
                    STKEIHIURIAGEKOZA = line.StKeihiUriageKoza,
                    STTORIHIKISAKIKOZA = line.StTorihikisakiKoza,
                    STNAIBUURIAGE = line.StNaibuUriage,
                    CDEOSTOKUISAKIHIMOZUKE = line.CdEosTokuisakiHimozuke,
                    CDAITETOKUISAKI = line.CdAiteTokuisaki,
                    NMAITEKAISHA = line.NmAiteKaisha,
                    NMAITEKAISHAYOMI = line.NmAitekaishaYomi,
                    CDAITEKAISHA = line.CdAiteKaisha,
                    NMAITESHOTEN = line.NmAiteShoten,
                    NMAITESHOTENYOMI = line.NmAiteShotenYomi,
                    STTESURYO = line.StTesuryo,
                    CDBUMONKAISHA = line.CdBumonKaisha,
                    STCHOKKATSUFC = line.StChokkatsuFc,
                    NMTOKUISAKIRYAKUSHO = line.NmTokuisakiRyakusho,
                    FGHATCHUYOTEISUSAKUSEIUMU = line.FgHatchuyoteisuSakuseiUmu,
                    FGGENTENKAHI = line.FgGentenKahi,
                    STORDERHYOUMU = line.StOrderhyoUmu,
                    CDORDERHYOGYOMUKYOTEN = line.CdOrderHyoGyomuKyoten,
                    STGYOMUSYSTEM = line.StGyomuSystem,
                    STMISESHUBETSU = line.StMiseShubetsu,
                    DTOYAKAISHI = line.DtOyaKaishi,
                    NMJUSHOYOMI2 = line.NmJushoYomi2,
                    NMKAISHA = line.NmKaisha,
                    NMKAISHAYOMI = line.NmKaishaYomi,
                    NMRYAKUSHOKAISHA = line.NmRyakushoKaisha,
                    NMRYAKUSHOKAISHAYOMI = line.NmRyakushoKaishaYomi,
                    NMTOKUISAKIRYAKUSHOYOMI = line.NmTokuisakiRyakushoYomi,
                    STSANDHATCHU = line.StSandHatchu,
                    STUEHARILABELHYOJI = line.StUehariLabelHyoji,
                    STGYOMUSHORIPATTERN = line.StGyomuShoriPattern,
                    CDHAISHOBIN = line.CdHaishoBin,
                    NUENKYORILEADTIME = line.NuEnkyoriLeadtime ?? 0,
                    IFIDO = line.IfIdo,
                    IFKEIDO = line.IfKeido,
                    NUKEMPINJIKAN = line.NuKempinJikan ?? 0,
                    TMNOHINKANOJI = line.TmNohinKanoJi,
                    TMNOHINKANOITARU = line.TmNohinKanoItaru,
                    CDHIGASHINIHONTEMBAN = line.CdHigashinihonTemban,
                    STURIKAKEKANRI = line.StUrikakeKanri,
                    IFSHUTTENKEIYAKUSAKI = line.IfShuttenKeiyakuSaki,
                    IFFCKEIYAKUSAKI = line.IfFcKeiyakuSaki,
                    IFBT1NUMBER = line.IfBt1Number,
                    IFBG3NUMBER = line.IfBg3Number,
                    CDTOKUISAKIGROUP9 = line.CdTokuisakiGroup9,
                    MNYOSHINGENDOGAKU = line.MnYoshinGendoGaku ?? 0,
                    DTZENKAIHENKO = line.DtZenkaiHenko,
                    STOKURIJOHAKKO = line.StOkurijoHakko,
                    STYUSENJUN = line.StYusenJun,
                    NUHAITA = line.NuHaita ?? 0,
                    DTRENKEI = line.DtRenkei,
                    CDRITCHI = line.CdRitchi,
                    CDTANTOSHA = line.CdTantosha,
                    DTKEIHONYUKIN = line.DtKeiHoNyukin,
                    MNKEIHOHOSHOKINGAKU = line.MnKeiHoHoshoKingaku ?? 0,
                    NMKEIHOJISHATANTO = line.NmKeiHoJishatanto,
                    NMKEIHOAITETANTO = line.NmKeiHoAiteTanto,
                    DTKAIHONYUKIN = line.DtKaiHoNyukin,
                    MNKAIHOHOSHOKINGAKU = line.MnKaiHoHoshoKingaku ?? 0,
                    NMKAIHOJISHATANTO = line.NmKaiHoJishatanto,
                    NMKAIHOAITETANTO = line.NmKaiHoAitetanto,
                    NMSHUKKALABELTENMEI = line.NmShukkalabelTenmei,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;
            }

            return importedCount;
        }

        private async Task<IEnumerable<TokuisakiFileLine>> ReadFileAsync(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<TokuisakiFileLine>();

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

                        var line = csv.GetRecord<TokuisakiFileLine>();
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
