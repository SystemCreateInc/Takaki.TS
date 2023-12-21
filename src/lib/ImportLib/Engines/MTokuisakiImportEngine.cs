
using CsvHelper;
using CsvHelper.Configuration;
using DbLib;
using DbLib.Defs;
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
    public class MTokuisakiImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> TargetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MTokuisakiImportEngine>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

        public MTokuisakiImportEngine(InterfaceFile interfaceFile)
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
                repo.DeleteTokuisakiData();

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

        public InterfaceFile GetInterfaceFile()
        {
            return _interfaceFile with { FileName = ImportFilePath };
        }

        public Task<bool> SetSameDist(CancellationToken token)
        {
            // 処理無し
            return Task.Run(() => true);
        }

        private int InsertData(DataImportController controller, IEnumerable<TokuisakiFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

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

                if ((importedCount % 33) == 0)
                {
                    controller.NotifyProgress("取込中", importedCount, datas.Count());
                }
            }

            return importedCount;
        }

        private IEnumerable<TokuisakiFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<TokuisakiFileLine>();

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
