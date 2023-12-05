
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
    public class MHimmokuImportEngine : IImportEngine
    {
        public DataType DataType => _interfaceFile.DataType;

        public string DataName => _interfaceFile.Name;

        public string ImportFilePath { get; set; } = string.Empty;

        public List<ImportFileInfo> _targetImportFiles { get; private set; } = new List<ImportFileInfo>();

        private InterfaceFile _interfaceFile;
        private ScopeLogger _logger = new ScopeLogger<MHimmokuImportEngine>();

        public IEnumerable<SameDistInfo> SameDistInfos { get; set; } = Enumerable.Empty<SameDistInfo>();

        public MHimmokuImportEngine(InterfaceFile interfaceFile)
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

        public async Task<List<ImportResult>> ImportAsync(CancellationToken token)
        {
            return await Task.Run(() => Import(token));
        }

        public List<ImportResult> Import(CancellationToken token)
        {
            using (var repo = new ImportRepository())
            {
                var importResults = new List<ImportResult>();
                var importDatas = new List<HimmokuFileLine>();
                repo.DeleteExpiredHimmokuData();

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

        private int InsertData(IEnumerable<HimmokuFileLine> datas, ImportRepository repo, CancellationToken token)
        {
            var importedCount = 0;
            foreach (var line in datas)
            {
                token.ThrowIfCancellationRequested();

                repo.Insert(new DbLib.DbEntities.TBMHIMMOKUEntity
                {
                    CDHIMBAN = line.CdHimban,                    
                    DTTEKIYOKAISHI = line.DtTekiyokaishi,
                    DTTEKIYOMUKO = line.DtTekiyomuko,
                    DTTOROKUNICHIJI = line.DtTorokuNichiji,
                    DTKOSHINNICHIJI = line.DtKoshinNichiji,
                    CDHENKOSHA = line.CdHenkosha,
                    NMHINSEISHIKIMEI = line.NmHinseiShikimei,
                    NMHINHYOKIMEI = line.NmHinHyokimei,
                    NMHINKANA = line.NmHinkana,
                    NMHINKANAHANKAKU = line.NmHinKanaHankaku,
                    NMHINRYAKUSHO = line.NmHinRyakusho,
                    CDKAIHATSUHIMBAN = line.CdKaihatsuHimban,
                    NMPOSTOROKUHIMMOKU = line.NmPosTorokuHimmoku,
                    STHIMMOKUTYPE = line.StHimmokuType,
                    STBUNRUIDAI = line.StBunruiDai,
                    STBUNRUICHU = line.StBunruiChu,
                    STBUNRUISHO = line.StBunruiSho,
                    STHINKANKANRI = line.StHinkanKanri,
                    STHIMMOKUKANRI = line.StHimmokuKanri,
                    CDSHUYAKUHIMBAN = line.CdShuyakuHimban,
                    NMSHUYAKUHIN = line.NmShuyakuHin,
                    STNBPB = line.StNbPb,
                    NMPBTOKUISAKI = line.NmPbtTokuisaki,
                    STREGI = line.StRegi,
                    PRHYOJUNKOURIHONTAI = line.PrHyojunKouriHontai ?? 0,
                    PRHYOJUNKOURIZEIKOMI = line.PrHyojunKouriZeikomi ?? 0,
                    DTJUCHUKAISHI = line.DtJuchuKaishi,
                    DTHAMBAIKAISHI = line.DtHambaiKaishi,
                    DTHAMBAISHURYO = line.DtHambaiShuryo,
                    DTJUCHUSHURYO = line.DtJuchuShuryo,
                    DTHEMPINSHURYO = line.DtHempinShuryo,
                    STHIMMOKU = line.StHimmoku,
                    STSENTAKU = line.StSentaku,
                    STNISSUJIKANSHOHI = line.StNissuJikanShohi,
                    STNISSUJIKANSHIYOU = line.StNissujikanShiyou,
                    STNISSUJIKANHANBAI = line.StNissujikanHanbai,
                    DTSHOMIKIGENREITO = line.DtShomiKigenReito ?? 0,
                    DTSHOHIKIGENKAKI = line.DtShohiKigenKaki ?? 0,
                    DTSHOHIKIGENTOKI = line.DtShohiKigenToki ?? 0,
                    DTSHIYOKIGENKAKI = line.DtShiyoKigenKaki ?? 0,
                    DTSHIYOKIGENTOKI = line.DtShiyoKigenToki ?? 0,
                    DTHAMBAIKIGENKAKI = line.DtHambaiKigenKaki ?? 0,
                    DTHAMBAIKIGENTOKI = line.DtHambaiKigenToki ?? 0,
                    DTKAKIKAISHI = line.DtKakiKaishi,
                    DTKAKISHURYO = line.DtKakiShuryo,
                    DTTOKIKAISHI = line.DtTokiKaishi,
                    DTTOKISHURYO = line.DtTokiShuryo,
                    DTUKEIRENATSUFROM = line.DtUkeireNatsuFrom ?? 0,
                    DTUKEIRENATSUTO = line.DtUkeireNatsuTo ?? 0,
                    DTUKEIREFUYUFROM = line.DtUkeireFuyuFrom ?? 0,
                    DTUKEIREFUYUTO = line.DtUkeireFuyuTo ?? 0,
                    STRYUTSUONDOTAI = line.StRyutsuOndotai,
                    STHAMBAIONDOTAI = line.StHambaiOndotai,
                    IFKAKAKUHYOJI = line.IfKakakuHyoji,
                    QTSET = line.QtSet ?? 0,
                    CDTORIATSUKAITANI = line.CdTOriatsukaiTani,
                    CDHYOJUNTANI = line.CdHyojunTani,
                    CDSETTANI = line.CdSetTani,
                    NUHYOJUNTORIATSUKAIKEISU = line.NuHyojunToriatsukaiKeisu ?? 0,
                    CDKOURITANI = line.CdKouriTani,
                    NUKOURIHENKANKEISU = line.NuKouriHenkanKeisu ?? 0,
                    CDHEMPINTANI = line.CdHempinTani,
                    NUHEMPINHENKANKEISU = line.NuHempinHenkanKeisu ?? 0,
                    CDHATCHUTANI = line.CdHatchuTani,
                    NUHATCHUHENKANKEISU = line.NuHatchuhenkanKeisu ?? 0,
                    NUHYOJUNHATCHUKEISU = line.NuHyojunHatchuKeisu ?? 0,
                    QTHATCHUIRISU = line.QtHatchuIrisu ?? 0,
                    CDHATCHUIRISUTANI = line.CdHatchuIrisuTani,
                    STFUTEIKAN = line.StFuteikan,
                    PROROSHI = line.PrOroshi ?? 0,
                    CDSEIZOKAISHA = line.CdSeizoKaisha,
                    CDSEIZOKAISHA2 = line.CdSeizoKaisha2,
                    NUSEIZOKAISU = line.NuSeizoKaisu ?? 0,
                    PRTEMPONAIFURIKAE = line.PrTemponaiFurikae ?? 0,
                    PRTEMPOKANFURIKAE = line.PrTempokanFurikae ?? 0,
                    PREIGYOWATASHI1 = line.PrEigyowatashi1 ?? 0,
                    PREIGYOWATASHI2 = line.PrEigyowatashi2 ?? 0,
                    PREIGYOWATASHI3 = line.PrEigyowatashi3 ?? 0,
                    PREIGYOWATASHI4 = line.PrEigyowatashi4 ?? 0,
                    PREIGYOWATASHI5 = line.PrEigyowatashi5 ?? 0,
                    PRDOITSUCOMPANYKOJOKAN = line.PrDoitsuCompanyKojokan ?? 0,
                    PRBETSUCOMPANYKOJOKAN = line.PrBetsuCompanyKojokan ?? 0,
                    PRSURIFUTOB = line.PrSurifutoB ?? 0,
                    PRSEISANSHIIRE = line.PrSeisanShiire ?? 0,
                    PRAQSHYOJUNTANKA = line.PrAqsHyojunTanka ?? 0,
                    STUMPANYOKISHUBETSU = line.StUmpanYokiShubetsu,
                    QTUMPANYOKIHAKOIRISU = line.QtUmpanyokiHakoIrisu ?? 0,
                    CDJAN = line.CdJan,
                    CDBUTSURYUKANRI = line.CdButsuryuKanri,
                    CDKOSEIJOSURYOTANI = line.CdKoseijoSuryoTani,
                    CDKOSEIJOJURYOTANI = line.CdKoseijoJuryoTani,
                    CDKOSEIJOYORYOTANI = line.CdKoseijoYoryoTani,
                    CDKOSEIJONAGASATANI = line.CdKoseijoNagasaTani,
                    NUKOSEIJOSURYOHENKANKEISU = line.NuKoseijoSuryoHenkankeisu ?? 0,
                    NUKOSEIJOJURYOHENKANKEISU = line.NuKoseijoJuryoHenkanKeisu ?? 0,
                    NUKOSEIJOSURYONAGASAKEISU = line.NuKoseijosuryonagasakeisu ?? 0,
                    NUKOSEIJOJURYONAGASAKEISU = line.NuKoseijojuryonagasakeisu ?? 0,
                    CDGENKAKEISANTANI = line.CdGenkakeisantani,
                    NUHYOJUNGENKAKEISU = line.NuHyojungenkakeisu ?? 0,
                    NUHYOJUNKOSEIJOSURYOKEISU = line.NuHyojunkoseijosuryokeisu ?? 0,
                    NUHYOJUNKOSEIJOJURYOKEISU = line.NuHyojunkoseijojuryokeisu ?? 0,
                    NUHYOJUNKOSEIJOYORYOKEISU = line.NuHyojunkoseijoyoryokeisu ?? 0,
                    NUHYOJUNKOSEIJONAGASAKEISU = line.NuHyojunkoseijonagasakeisu ?? 0,
                    STTOMEGATA = line.StTomegata,
                    IFNISUGATA = line.IfNisugata,
                    CDGENSANKOKU = line.CdGensankoku,
                    STSOBAHIN = line.StSobahin,
                    STLOCALHIN = line.StLocalhin,
                    PRKG = line.PrKg ?? 0,
                    IFZAISHITSU = line.IfZaishitsu,
                    NUNAGASA = line.NuNagasa ?? 0,
                    NUNAGASAJOGEN = line.NuNagasajogen ?? 0,
                    NUNAGASAKAGEN = line.NuNagasakagen ?? 0,
                    CDNAGASATANI = line.CdNagasatani,
                    NUHABA = line.NuHaba ?? 0,
                    NUHABAJOGEN = line.NuHabajogen ?? 0,
                    NUHABAKAGEN = line.NuHabakagen ?? 0,
                    CDHABATANI = line.CdHabatani,
                    NUTAKASA = line.NuTakasa ?? 0,
                    NUTAKASAJOGEN = line.NuTakasajogen ?? 0,
                    NUTAKASAKAGEN = line.NuTakasakagen ?? 0,
                    CDTAKASATANI = line.CdTAkasatani,
                    NUJURYO = line.NuJuryo ?? 0,
                    NUJURYOJOGEN = line.NuJuryojogen ?? 0,
                    NUJURYOKAGEN = line.NuJuryokagen ?? 0,
                    NUSOJURYO = line.NuSojuryo ?? 0,
                    CDJURYOTANI = line.CdJuryotani,
                    NUSOSO = line.NuSoso ?? 0,
                    NUTSUTSUCHOKKEI = line.NuTsutsuchokkei ?? 0,
                    NUSAIDAICHOKKEI = line.NuSaidaichokkei ?? 0,
                    STMAKIHOKO = line.StMakihoko,
                    CDFUKUROIRISUTANI = line.CdFukuroirisutani,
                    QTFUKUROIRISU = line.QtFukuroirisu ?? 0,
                    IFHAMBAIYOHOZAI = line.IfHambaiyohozai,
                    FGSHOMISHOHIKIGENYOFUYO = line.Fgshomishohikigenyofuyo,
                    FGGENZAIRYOYOFUYO = line.Fggenzairyoyofuyo,
                    STSENTAKU100G = line.StSentaku100g,
                    FGEIYOSEIBUNHYOJI = line.Fgeiyoseibunhyoji,
                    IFSHIKIBETSUHYOJI = line.IfShikibetsuhyoji,
                    IFPRICECARDHYOJINAIYO = line.IfPricecardhyojinaiyo,
                    IFPRICECARDGAZO1 = line.IfPricecardgazo1,
                    IFPRICECARDGAZO2 = line.IfPricecardgazo2,
                    IFPRICECARDGAZO3 = line.IfPricecardgazo3,
                    NUNAIYOJURYO = line.NuNaiyojuryo ?? 0,
                    CDNAIYOJURYO = line.CdNaiyojuryo,
                    NUKOKEIJURYO = line.NuKokeijuryo ?? 0,
                    CDKEKEIJURYO = line.CdKekeijuryo,
                    NUHOKANJOKENKAIFUMAE = line.NuHokanjokenkaifumae,
                    NUHOKANJOKENKAIFUGO = line.NuHokanjokenkaifugo,
                    DTKAIFUGOSHIYOKIGENKAKI = line.DtKaifugoshiyokigenkaki,
                    STKAIFUGOSHIYOKIGENKAKI = line.StKaifugoshiyokigenkaki,
                    DTKAIFUGOSHIYOKIGENTOKI = line.DtKaifugoshiyokigentoki,
                    STKAIFUGOSHIYOKIGENTOKI = line.StKaifugoshiyokigentoki,
                    CDYAKIIRONERAISHOKU = line.CdYakiironeraishoku,
                    CDYAKIIROKAGEN = line.CdYakiirokagen,
                    CDYAKIIROJOGEN = line.CdYakiirojogen,
                    CDSAISHOSEZOTANI = line.CdSaishosezotani,
                    QTSAISHOSEIZOTANI = line.QtSaishoseizotani ?? 0,
                    FGRITADO = line.Fgritado,
                    CDSAIJI = line.CdSaiji,
                    STSHOHINSEIHIN = line.StShohinseihin,
                    STKEIHIHINSHU = line.StKeihihinshu,
                    STSHIIRE = line.StShiire,
                    STSHOHINBUNRUI = line.StShohinbunrui,
                    FGGENTEIUMU = line.Fggenteiumu,
                    FGWEBJYUCHUKANRI = line.Fgwebjyuchukanri,
                    NUZEIRITSU = line.NuZeiritsu ?? 0,
                    STZEIRITSU = line.StZeiritsu,
                    FGZAIKOKANRI = line.Fgzaikokanri,
                    STGR = line.StGr,
                    STHASUSHOHIZEI = line.StHasushohizei,
                    STKANRIHIMMOKU = line.StKanrihimmoku,
                    CDOYAHIMBAN = line.CdOyahimban,
                    STKAZEI = line.StKazei,
                    STURIAGEBUNRUI = line.StUriagebunrui,
                    STDAI = line.StDai,
                    STCHU = line.StChu,
                    STSHO = line.StSho,
                    STHIMMOKUBUNRUI = line.StHimmokubunrui,
                    STNOHIMBIFUTEI = line.StNohimbifutei,
                    STEIGYOURIAGE = line.StEigyouriage,
                    STEIGYOKANRI = line.StEigyokanri,
                    STSEIHINSHOHIN = line.StSeihinshohin,
                    STSHOHINCATEGORY = line.StShohincategory,
                    STKIJISHURUI = line.StKijishurui,
                    QTBARATANI = line.QtBaratani,
                    NUHYOJUNBARAKEISU = line.NuHyojunbarakeisu ?? 0,
                    CDRETAILHINGUN = line.CdRetailhingun,
                    CDTANTOBUMON = line.CdTAntobumon,
                    NUHAMBAINISSU = line.NuHambainissu ?? 0,
                    CDHYOJUNSHIIRESAKI = line.CdHyojunshiiresaki,
                    PRHYOJUNTANKA = line.PrHyojuntanka ?? 0,
                    PRRIRONGENKA = line.PrRirongenka ?? 0,
                    PRHYOJUNGENKA = line.PrHyojungenka ?? 0,
                    PRSEIZOGENKA = line.PrSeiZogenka ?? 0,
                    STKOJO = line.StKojo,
                    STRETAIL = line.StRetail,
                    STOROSHI = line.StOroshi,
                    STHIMMOKUKAZEI = line.StHimmokukazei,
                    STCASE = line.StCase,
                    IFHIMMOKUGAZO1 = line.IfHimmokugazo1,
                    IFHIMMOKUGAZO2 = line.IfHimmokugazo2,
                    IFHIMMOKUGAZO3 = line.IfHimmokugazo3,
                    IFHIMMOKUGAZO4 = line.IfHimmokugazo4,
                    CDSHOHINBUNRUI9 = line.CdShohinbunrui9,
                    DTZENKAIHENKO = line.DtZenkaihenko,
                    DTOYAKAISHI = line.DtOyakaishi,
                    DTHATSUBAISHOUNIN = line.DtHatsubaishounin,
                    FGSHOUHINSEKKEIIRAISHOKAHI = line.FgShouhinsekkeiiraishokahi,
                    FGSHOUHINSEKKEISHOKAHI = line.FgShouhinsekkeishokahi,
                    FGKAIHATSUSHOUNINSHOKAHI = line.FgKaihatsushouninshokahi,
                    FGHATSUBAISHOUNINSHOKAHI = line.FgHatsubaishouninshokahi,
                    FGHOUZAIIRAISHOKAHI = line.FgHouzaiiraishokahi,
                    FGHINMOKUTOUROKUIRAISHOKAHI = line.FgHinmokutourokuiraishokahi,
                    CDPROCESS = line.CdProcess,
                    STBUTSURYUSEIGYO = line.StButsuryuseigyo,
                    CDKANRICOMPANY = line.CdKanricompany,
                    IFSHIKIBETSUHYOJIPLA = line.IfShikibetsuhyojipla,
                    DTRENKEI = line.DtRenkei,
                    NUHAITA = line.NuHaita ?? 0,
                    CDSEIZOHIMBAN = line.CdSeizohimban,
                    CDFAHIMBAN = line.CdFahimban,
                    CDNISHIHIMBAN = line.CdNishihimban,
                    CDHIGASHIHIMBAN = line.CdHigashihimban,
                    CDRETAILHIMBAN = line.CdRetailhimban,
                    CDHIROSHIMAANHIMBAN = line.CdHiroshimaanhimban,
                    CDKODANISAHIMBAN = line.CdKodanisahimban,
                    CDKOBAIHIMBAN = line.CdKobaihimban,
                    CDSSSHIMBAN = line.CdSsshimban,
                    STSHONIN = line.StShonin,
                    IFSEKKEISHOBIKO1 = line.IfSekkeishobiko1,
                    IFSEKKEISHOBIKO2 = line.IfSekkeishobiko2,
                    IFSEKKEISHOBIKO3 = line.IfSekkeishobiko3,
                    FGHOGOJOGAI = line.FgHogojogai,
                    CDQANumeric = line.CdQanumeric,
                    CDITEMGTIN = line.CdItemgtin,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                });

                ++importedCount;
            }

            return importedCount;
        }

        private IEnumerable<HimmokuFileLine> ReadFile(CancellationToken token, string targetImportFilePath)
        {
            Syslog.Debug($"Read {DataName} file");
            var datas = new List<HimmokuFileLine>();

            try
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    // fixme:実データ項目不足の為、暫定的に許容とする
                    MissingFieldFound = null,
                };

                using (var reader = new StreamReader(targetImportFilePath, Encoding.GetEncoding("shift_jis")))
                using (var csv = new CsvReader(reader, config))
                {
                    while (csv.Read())
                    {
                        token.ThrowIfCancellationRequested();

                        try
                        {
                            var line = csv.GetRecord<HimmokuFileLine>();
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
