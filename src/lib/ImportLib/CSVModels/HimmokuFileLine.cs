using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.CSVModels
{
    public class HimmokuFileLine
    {
        [Index(0)]
        public string CdHimban { get; set; } = string.Empty;
        [Index(1)]
        public string DtTekiyokaishi { get; set; } = string.Empty;
        [Index(2)]
        public string DtTekiyomuko { get; set; } = string.Empty;
        [Index(3)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(4)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(5)]
        public string CdHenkosha { get; set; } = string.Empty;
        [Index(6)]
        public string NmHinseiShikimei { get; set; } = string.Empty;
        [Index(7)]
        public string NmHinHyokimei { get; set; } = string.Empty;
        [Index(8)]
        public string NmHinkana { get; set; } = string.Empty;
        [Index(9)]
        public string NmHinKanaHankaku { get; set; } = string.Empty;
        [Index(10)]
        public string NmHinRyakusho { get; set; } = string.Empty;
        [Index(11)]
        public string CdKaihatsuHimban { get; set; } = string.Empty;
        [Index(12)]
        public string NmPosTorokuHimmoku { get; set; } = string.Empty;
        [Index(13)]
        public string StHimmokuType { get; set; } = string.Empty;
        [Index(14)]
        public string StBunruiDai { get; set; } = string.Empty;
        [Index(15)]
        public string StBunruiChu { get; set; } = string.Empty;
        [Index(16)]
        public string StBunruiSho { get; set; } = string.Empty;
        [Index(17)]
        public string StHinkanKanri { get; set; } = string.Empty;
        [Index(18)]
        public string StHimmokuKanri { get; set; } = string.Empty;
        [Index(19)]
        public string CdShuyakuHimban { get; set; } = string.Empty;
        [Index(20)]
        public string NmShuyakuHin { get; set; } = string.Empty;
        [Index(21)]
        public string StNbPb { get; set; } = string.Empty;
        [Index(22)]
        public string NmPbtTokuisaki { get; set; } = string.Empty;
        [Index(23)]
        public string StRegi { get; set; } = string.Empty;
        [Index(24)]
        public decimal? PrHyojunKouriHontai { get; set; }
        [Index(25)]
        public decimal? PrHyojunKouriZeikomi { get; set; }
        [Index(26)]
        public string DtJuchuKaishi { get; set; } = string.Empty;
        [Index(27)]
        public string DtHambaiKaishi { get; set; } = string.Empty;
        [Index(28)]
        public string DtHambaiShuryo { get; set; } = string.Empty;
        [Index(29)]
        public string DtJuchuShuryo { get; set; } = string.Empty;
        [Index(30)]
        public string DtHempinShuryo { get; set; } = string.Empty;
        [Index(31)]
        public string StHimmoku { get; set; } = string.Empty;
        [Index(32)]
        public string StSentaku { get; set; } = string.Empty;
        [Index(33)]
        public string StNissuJikanShohi { get; set; } = string.Empty;
        [Index(34)]
        public string StNissujikanShiyou { get; set; } = string.Empty;
        [Index(35)]
        public string StNissujikanHanbai { get; set; } = string.Empty;
        [Index(36)]
        public decimal? DtShomiKigenReito { get; set; }
        [Index(37)]
        public decimal? DtShohiKigenKaki { get; set; }
        [Index(38)]
        public decimal? DtShohiKigenToki { get; set; }
        [Index(39)]
        public decimal? DtShiyoKigenKaki { get; set; }
        [Index(40)]
        public decimal? DtShiyoKigenToki { get; set; }
        [Index(41)]
        public decimal? DtHambaiKigenKaki { get; set; }
        [Index(42)]
        public decimal? DtHambaiKigenToki { get; set; }
        [Index(43)]
        public string DtKakiKaishi { get; set; } = string.Empty;
        [Index(44)]
        public string DtKakiShuryo { get; set; } = string.Empty;
        [Index(45)]
        public string DtTokiKaishi { get; set; } = string.Empty;
        [Index(46)]
        public string DtTokiShuryo { get; set; } = string.Empty;
        [Index(47)]
        public decimal? DtUkeireNatsuFrom { get; set; }
        [Index(48)]
        public decimal? DtUkeireNatsuTo { get; set; }
        [Index(49)]
        public decimal? DtUkeireFuyuFrom { get; set; }
        [Index(50)]
        public decimal? DtUkeireFuyuTo { get; set; }
        [Index(51)]
        public string StRyutsuOndotai { get; set; } = string.Empty;
        [Index(52)]
        public string StHambaiOndotai { get; set; } = string.Empty;
        [Index(53)]
        public string IfKakakuHyoji { get; set; } = string.Empty;
        [Index(54)]
        public decimal? QtSet { get; set; }
        [Index(55)]
        public string CdTOriatsukaiTani { get; set; } = string.Empty;
        [Index(56)]
        public string CdHyojunTani { get; set; } = string.Empty;
        [Index(57)]
        public string CdSetTani { get; set; } = string.Empty;
        [Index(58)]
        public decimal? NuHyojunToriatsukaiKeisu { get; set; }
        [Index(59)]
        public string CdKouriTani { get; set; } = string.Empty;
        [Index(60)]
        public decimal? NuKouriHenkanKeisu { get; set; }
        [Index(61)]
        public string CdHempinTani { get; set; } = string.Empty;
        [Index(62)]
        public decimal? NuHempinHenkanKeisu { get; set; }
        [Index(63)]
        public string CdHatchuTani { get; set; } = string.Empty;
        [Index(64)]
        public decimal? NuHatchuhenkanKeisu { get; set; }
        [Index(65)]
        public decimal? NuHyojunHatchuKeisu { get; set; }
        [Index(66)]
        public decimal? QtHatchuIrisu { get; set; }
        [Index(67)]
        public string CdHatchuIrisuTani { get; set; } = string.Empty;
        [Index(68)]
        public string StFuteikan { get; set; } = string.Empty;
        [Index(69)]
        public decimal? PrOroshi { get; set; }
        [Index(70)]
        public string CdSeizoKaisha { get; set; } = string.Empty;
        [Index(71)]
        public string CdSeizoKaisha2 { get; set; } = string.Empty;
        [Index(72)]
        public decimal? NuSeizoKaisu { get; set; }
        [Index(73)]
        public decimal? PrTemponaiFurikae { get; set; }
        [Index(74)]
        public decimal? PrTempokanFurikae { get; set; }
        [Index(75)]
        public decimal? PrEigyowatashi1 { get; set; }
        [Index(76)]
        public decimal? PrEigyowatashi2 { get; set; }
        [Index(77)]
        public decimal? PrEigyowatashi3 { get; set; }
        [Index(78)]
        public decimal? PrEigyowatashi4 { get; set; }
        [Index(79)]
        public decimal? PrEigyowatashi5 { get; set; }
        [Index(80)]
        public decimal? PrDoitsuCompanyKojokan { get; set; }
        [Index(81)]
        public decimal? PrBetsuCompanyKojokan { get; set; }
        [Index(82)]
        public decimal? PrSurifutoB { get; set; }
        [Index(83)]
        public decimal? PrSeisanShiire { get; set; }
        [Index(84)]
        public decimal? PrAqsHyojunTanka { get; set; }
        [Index(85)]
        public string StUmpanYokiShubetsu { get; set; } = string.Empty;
        [Index(86)]
        public decimal? QtUmpanyokiHakoIrisu { get; set; }
        [Index(87)]
        public string CdJan { get; set; } = string.Empty;
        [Index(88)]
        public string CdButsuryuKanri { get; set; } = string.Empty;
        [Index(89)]
        public string CdKoseijoSuryoTani { get; set; } = string.Empty;
        [Index(90)]
        public string CdKoseijoJuryoTani { get; set; } = string.Empty;
        [Index(91)]
        public string CdKoseijoYoryoTani { get; set; } = string.Empty;
        [Index(92)]
        public string CdKoseijoNagasaTani { get; set; } = string.Empty;
        [Index(93)]
        public decimal? NuKoseijoSuryoHenkankeisu { get; set; }
        [Index(94)]
        public decimal? NuKoseijoJuryoHenkanKeisu { get; set; }
        [Index(95)]
        public decimal? NuKoseijosuryonagasakeisu { get; set; }
        [Index(96)]
        public decimal? NuKoseijojuryonagasakeisu { get; set; }
        [Index(97)]
        public string CdGenkakeisantani { get; set; } = string.Empty;
        [Index(98)]
        public decimal? NuHyojungenkakeisu { get; set; }
        [Index(99)]
        public decimal? NuHyojunkoseijosuryokeisu { get; set; }
        [Index(100)]
        public decimal? NuHyojunkoseijojuryokeisu { get; set; }
        [Index(101)]
        public decimal? NuHyojunkoseijoyoryokeisu { get; set; }
        [Index(102)]
        public decimal? NuHyojunkoseijonagasakeisu { get; set; }
        [Index(103)]
        public string StTomegata { get; set; } = string.Empty;
        [Index(104)]
        public string IfNisugata { get; set; } = string.Empty;
        [Index(105)]
        public string CdGensankoku { get; set; } = string.Empty;
        [Index(106)]
        public string StSobahin { get; set; } = string.Empty;
        [Index(107)]
        public string StLocalhin { get; set; } = string.Empty;
        [Index(108)]
        public decimal? PrKg { get; set; }
        [Index(109)]
        public string IfZaishitsu { get; set; } = string.Empty;
        [Index(110)]
        public decimal? NuNagasa { get; set; }
        [Index(111)]
        public decimal? NuNagasajogen { get; set; }
        [Index(112)]
        public decimal? NuNagasakagen { get; set; }
        [Index(113)]
        public string CdNagasatani { get; set; } = string.Empty;
        [Index(114)]
        public decimal? NuHaba { get; set; }
        [Index(115)]
        public decimal? NuHabajogen { get; set; }
        [Index(116)]
        public decimal? NuHabakagen { get; set; }
        [Index(117)]
        public string CdHabatani { get; set; } = string.Empty;
        [Index(118)]
        public decimal? NuTakasa { get; set; }
        [Index(119)]
        public decimal? NuTakasajogen { get; set; }
        [Index(120)]
        public decimal? NuTakasakagen { get; set; }
        [Index(121)]
        public string CdTAkasatani { get; set; } = string.Empty;
        [Index(122)]
        public decimal? NuJuryo { get; set; }
        [Index(123)]
        public decimal? NuJuryojogen { get; set; }
        [Index(124)]
        public decimal? NuJuryokagen { get; set; }
        [Index(125)]
        public decimal? NuSojuryo { get; set; }
        [Index(126)]
        public string CdJuryotani { get; set; } = string.Empty;
        [Index(127)]
        public decimal? NuSoso { get; set; }
        [Index(128)]
        public decimal? NuTsutsuchokkei { get; set; }
        [Index(129)]
        public decimal? NuSaidaichokkei { get; set; }
        [Index(130)]
        public string StMakihoko { get; set; } = string.Empty;
        [Index(131)]
        public string CdFukuroirisutani { get; set; } = string.Empty;
        [Index(132)]
        public decimal? QtFukuroirisu { get; set; }
        [Index(133)]
        public string IfHambaiyohozai { get; set; } = string.Empty;
        [Index(134)]
        public string Fgshomishohikigenyofuyo { get; set; } = string.Empty;
        [Index(135)]
        public string Fggenzairyoyofuyo { get; set; } = string.Empty;
        [Index(136)]
        public string StSentaku100g { get; set; } = string.Empty;
        [Index(137)]
        public string Fgeiyoseibunhyoji { get; set; } = string.Empty;
        [Index(138)]
        public string IfShikibetsuhyoji { get; set; } = string.Empty;
        [Index(139)]
        public string IfPricecardhyojinaiyo { get; set; } = string.Empty;
        [Index(140)]
        public string IfPricecardgazo1 { get; set; } = string.Empty;
        [Index(141)]
        public string IfPricecardgazo2 { get; set; } = string.Empty;
        [Index(142)]
        public string IfPricecardgazo3 { get; set; } = string.Empty;
        [Index(143)]
        public decimal? NuNaiyojuryo { get; set; }
        [Index(144)]
        public string CdNaiyojuryo { get; set; } = string.Empty;
        [Index(145)]
        public decimal? NuKokeijuryo { get; set; }
        [Index(146)]
        public string CdKekeijuryo { get; set; } = string.Empty;
        [Index(147)]
        public string NuHokanjokenkaifumae { get; set; } = string.Empty;
        [Index(148)]
        public string NuHokanjokenkaifugo { get; set; } = string.Empty;
        [Index(149)]
        public string DtKaifugoshiyokigenkaki { get; set; } = string.Empty;
        [Index(150)]
        public string StKaifugoshiyokigenkaki { get; set; } = string.Empty;
        [Index(151)]
        public string DtKaifugoshiyokigentoki { get; set; } = string.Empty;
        [Index(152)]
        public string StKaifugoshiyokigentoki { get; set; } = string.Empty;
        [Index(153)]
        public string CdYakiironeraishoku { get; set; } = string.Empty;
        [Index(154)]
        public string CdYakiirokagen { get; set; } = string.Empty;
        [Index(155)]
        public string CdYakiirojogen { get; set; } = string.Empty;
        [Index(156)]
        public string CdSaishosezotani { get; set; } = string.Empty;
        [Index(157)]
        public decimal? QtSaishoseizotani { get; set; }
        [Index(158)]
        public string Fgritado { get; set; } = string.Empty;
        [Index(159)]
        public string CdSaiji { get; set; } = string.Empty;
        [Index(160)]
        public string StShohinseihin { get; set; } = string.Empty;
        [Index(161)]
        public string StKeihihinshu { get; set; } = string.Empty;
        [Index(162)]
        public string StShiire { get; set; } = string.Empty;
        [Index(163)]
        public string StShohinbunrui { get; set; } = string.Empty;
        [Index(164)]
        public string Fggenteiumu { get; set; } = string.Empty;
        [Index(165)]
        public string Fgwebjyuchukanri { get; set; } = string.Empty;
        [Index(166)]
        public decimal? NuZeiritsu { get; set; }
        [Index(167)]
        public string StZeiritsu { get; set; } = string.Empty;
        [Index(168)]
        public string Fgzaikokanri { get; set; } = string.Empty;
        [Index(169)]
        public string StGr { get; set; } = string.Empty;
        [Index(170)]
        public string StHasushohizei { get; set; } = string.Empty;
        [Index(171)]
        public string StKanrihimmoku { get; set; } = string.Empty;
        [Index(172)]
        public string CdOyahimban { get; set; } = string.Empty;
        [Index(173)]
        public string StKazei { get; set; } = string.Empty;
        [Index(174)]
        public string StUriagebunrui { get; set; } = string.Empty;
        [Index(175)]
        public string StDai { get; set; } = string.Empty;
        [Index(176)]
        public string StChu { get; set; } = string.Empty;
        [Index(177)]
        public string StSho { get; set; } = string.Empty;
        [Index(178)]
        public string StHimmokubunrui { get; set; } = string.Empty;
        [Index(179)]
        public string StNohimbifutei { get; set; } = string.Empty;
        [Index(180)]
        public string StEigyouriage { get; set; } = string.Empty;
        [Index(181)]
        public string StEigyokanri { get; set; } = string.Empty;
        [Index(182)]
        public string StSeihinshohin { get; set; } = string.Empty;
        [Index(183)]
        public string StShohincategory { get; set; } = string.Empty;
        [Index(184)]
        public string StKijishurui { get; set; } = string.Empty;
        [Index(185)]
        public string QtBaratani { get; set; } = string.Empty;
        [Index(186)]
        public decimal? NuHyojunbarakeisu { get; set; }
        [Index(187)]
        public string CdRetailhingun { get; set; } = string.Empty;
        [Index(188)]
        public string CdTAntobumon { get; set; } = string.Empty;
        [Index(189)]
        public decimal? NuHambainissu { get; set; }
        [Index(190)]
        public string CdHyojunshiiresaki { get; set; } = string.Empty;
        [Index(191)]
        public decimal? PrHyojuntanka { get; set; }
        [Index(192)]
        public decimal? PrRirongenka { get; set; }
        [Index(193)]
        public decimal? PrHyojungenka { get; set; }
        [Index(194)]
        public decimal? PrSeiZogenka { get; set; }
        [Index(195)]
        public string StKojo { get; set; } = string.Empty;
        [Index(196)]
        public string StRetail { get; set; } = string.Empty;
        [Index(197)]
        public string StOroshi { get; set; } = string.Empty;
        [Index(198)]
        public string StHimmokukazei { get; set; } = string.Empty;
        [Index(199)]
        public string StCase { get; set; } = string.Empty;
        [Index(200)]
        public string IfHimmokugazo1 { get; set; } = string.Empty;
        [Index(201)]
        public string IfHimmokugazo2 { get; set; } = string.Empty;
        [Index(202)]
        public string IfHimmokugazo3 { get; set; } = string.Empty;
        [Index(203)]
        public string IfHimmokugazo4 { get; set; } = string.Empty;
        [Index(204)]
        public string CdShohinbunrui9 { get; set; } = string.Empty;
        [Index(205)]
        public string DtZenkaihenko { get; set; } = string.Empty;
        [Index(206)]
        public string DtOyakaishi { get; set; } = string.Empty;
        [Index(207)]
        public string DtHatsubaishounin { get; set; } = string.Empty;
        [Index(208)]
        public string FgShouhinsekkeiiraishokahi { get; set; } = string.Empty;
        [Index(209)]
        public string FgShouhinsekkeishokahi { get; set; } = string.Empty;
        [Index(210)]
        public string FgKaihatsushouninshokahi { get; set; } = string.Empty;
        [Index(211)]
        public string FgHatsubaishouninshokahi { get; set; } = string.Empty;
        [Index(212)]
        public string FgHouzaiiraishokahi { get; set; } = string.Empty;
        [Index(213)]
        public string FgHinmokutourokuiraishokahi { get; set; } = string.Empty;
        [Index(214)]
        public string CdProcess { get; set; } = string.Empty;
        [Index(215)]
        public string StButsuryuseigyo { get; set; } = string.Empty;
        [Index(216)]
        public string CdKanricompany { get; set; } = string.Empty;
        [Index(217)]
        public string IfShikibetsuhyojipla { get; set; } = string.Empty;
        [Index(218)]
        public string DtRenkei { get; set; } = string.Empty;
        [Index(219)]
        public decimal? NuHaita { get; set; }
        [Index(220)]
        public string CdItemgtin { get; set; } = string.Empty;
    }
}
