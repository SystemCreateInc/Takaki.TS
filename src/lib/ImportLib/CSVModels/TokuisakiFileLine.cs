using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportLib.CSVModels
{
    public class TokuisakiFileLine
    {
        [Index(0)]
        public string CdTokuisaki { get; set; } = string.Empty;
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
        public string NmTokuisaki { get; set; } = string.Empty;
        [Index(7)]
        public string NmTokuisakiYomi { get; set; } = string.Empty;
        [Index(8)]
        public string NmJusho1 { get; set; } = string.Empty;
        [Index(9)]
        public string NmJusho2 { get; set; } = string.Empty;
        [Index(10)]
        public string NmJushoYomi { get; set; } = string.Empty;
        [Index(11)]
        public string IfYubinBango { get; set; } = string.Empty;
        [Index(12)]
        public string IfDenwaBango { get; set; } = string.Empty;
        [Index(13)]
        public string IfKensakuyoDenwaBango { get; set; } = string.Empty;
        [Index(14)]
        public string IfFaxBango { get; set; } = string.Empty;
        [Index(15)]
        public string CdChiiki { get; set; } = string.Empty;
        [Index(16)]
        public string DtKaiten { get; set; } = string.Empty;
        [Index(17)]
        public string DtHeiten { get; set; } = string.Empty;
        [Index(18)]
        public string DtKeiyaku { get; set; } = string.Empty;
        [Index(19)]
        public string DtKaiyaku { get; set; } = string.Empty;
        [Index(20)]
        public string StChanel { get; set; } = string.Empty;
        [Index(21)]
        public string CdKankatsuEigyobumon { get; set; } = string.Empty;
        [Index(22)]
        public string CdTantoGyomukyoten { get; set; } = string.Empty;
        [Index(23)]
        public string StGyotai { get; set; } = string.Empty;
        [Index(24)]
        public string StTokuisakiShikibetsu { get; set; } = string.Empty;
        [Index(25)]
        public string StTokuisakiShubetsu { get; set; } = string.Empty;
        [Index(26)]
        public string NoOyashoten { get; set; } = string.Empty;
        [Index(27)]
        public string NoSoshiki1 { get; set; } = string.Empty;
        [Index(28)]
        public string NoSoshiki2 { get; set; } = string.Empty;
        [Index(29)]
        public string CdKeiyakuGroup { get; set; } = string.Empty;
        [Index(30)]
        public string NoKeiyakuSedai { get; set; } = string.Empty;
        [Index(31)]
        public string CdShodanGroup { get; set; } = string.Empty;
        [Index(32)]
        public string NoShodanSedai { get; set; } = string.Empty;
        [Index(33)]
        public string CdJuchuGroup { get; set; } = string.Empty;
        [Index(34)]
        public string NoJuchuSedai { get; set; } = string.Empty;
        [Index(35)]
        public string CdNohinGroup { get; set; } = string.Empty;
        [Index(36)]
        public string NoNohinSedai { get; set; } = string.Empty;
        [Index(37)]
        public string CdSeikyuGroup { get; set; } = string.Empty;
        [Index(38)]
        public string NoSeikyuSedai { get; set; } = string.Empty;
        [Index(39)]
        public string CdNyukinGroup { get; set; } = string.Empty;
        [Index(40)]
        public string NoNyukinSedai { get; set; } = string.Empty;
        [Index(41)]
        public string StKazei { get; set; } = string.Empty;
        [Index(42)]
        public string StShohizeiKeisan { get; set; } = string.Empty;
        [Index(43)]
        public string StNaibuUriageKoza { get; set; } = string.Empty;
        [Index(44)]
        public string StKeihiUriageKoza { get; set; } = string.Empty;
        [Index(45)]
        public string StTorihikisakiKoza { get; set; } = string.Empty;
        [Index(46)]
        public string StNaibuUriage { get; set; } = string.Empty;
        [Index(47)]
        public string CdEosTokuisakiHimozuke { get; set; } = string.Empty;
        [Index(48)]
        public string CdAiteTokuisaki { get; set; } = string.Empty;
        [Index(49)]
        public string NmAiteKaisha { get; set; } = string.Empty;
        [Index(50)]
        public string NmAitekaishaYomi { get; set; } = string.Empty;
        [Index(51)]
        public string CdAiteKaisha { get; set; } = string.Empty;
        [Index(52)]
        public string NmAiteShoten { get; set; } = string.Empty;
        [Index(53)]
        public string NmAiteShotenYomi { get; set; } = string.Empty;
        [Index(54)]
        public string StTesuryo { get; set; } = string.Empty;
        [Index(55)]
        public string CdBumonKaisha { get; set; } = string.Empty;
        [Index(56)]
        public string StChokkatsuFc { get; set; } = string.Empty;
        [Index(57)]
        public string NmTokuisakiRyakusho { get; set; } = string.Empty;
        [Index(58)]
        public string FgHatchuyoteisuSakuseiUmu { get; set; } = string.Empty;
        [Index(59)]
        public string FgGentenKahi { get; set; } = string.Empty;
        [Index(60)]
        public string StOrderhyoUmu { get; set; } = string.Empty;
        [Index(61)]
        public string CdOrderHyoGyomuKyoten { get; set; } = string.Empty;
        [Index(62)]
        public string StGyomuSystem { get; set; } = string.Empty;
        [Index(63)]
        public string StMiseShubetsu { get; set; } = string.Empty;
        [Index(64)]
        public string DtOyaKaishi { get; set; } = string.Empty;
        [Index(65)]
        public string NmJushoYomi2 { get; set; } = string.Empty;
        [Index(66)]
        public string NmKaisha { get; set; } = string.Empty;
        [Index(67)]
        public string NmKaishaYomi { get; set; } = string.Empty;
        [Index(68)]
        public string NmRyakushoKaisha { get; set; } = string.Empty;
        [Index(69)]
        public string NmRyakushoKaishaYomi { get; set; } = string.Empty;
        [Index(70)]
        public string NmTokuisakiRyakushoYomi { get; set; } = string.Empty;
        [Index(71)]
        public string StSandHatchu { get; set; } = string.Empty;
        [Index(72)]
        public string StUehariLabelHyoji { get; set; } = string.Empty;
        [Index(73)]
        public string StGyomuShoriPattern { get; set; } = string.Empty;
        [Index(74)]
        public string CdHaishoBin { get; set; } = string.Empty;
        [Index(75)]
        public decimal? NuEnkyoriLeadtime { get; set; } = 0;
        [Index(76)]
        public string IfIdo { get; set; } = string.Empty;
        [Index(77)]
        public string IfKeido { get; set; } = string.Empty;
        [Index(78)]
        public decimal? NuKempinJikan { get; set; } = 0;
        [Index(79)]
        public string TmNohinKanoJi { get; set; } = string.Empty;
        [Index(80)]
        public string TmNohinKanoItaru { get; set; } = string.Empty;
        [Index(81)]
        public string CdHigashinihonTemban { get; set; } = string.Empty;
        [Index(82)]
        public string StUrikakeKanri { get; set; } = string.Empty;
        [Index(83)]
        public string IfShuttenKeiyakuSaki { get; set; } = string.Empty;
        [Index(84)]
        public string IfFcKeiyakuSaki { get; set; } = string.Empty;
        [Index(85)]
        public string IfBt1Number { get; set; } = string.Empty;
        [Index(86)]
        public string IfBg3Number { get; set; } = string.Empty;
        [Index(87)]
        public string CdTokuisakiGroup9 { get; set; } = string.Empty;
        [Index(88)]
        public decimal? MnYoshinGendoGaku { get; set; } = 0;
        [Index(89)]
        public string DtZenkaiHenko { get; set; } = string.Empty;
        [Index(90)]
        public string StOkurijoHakko { get; set; } = string.Empty;
        [Index(91)]
        public string StYusenJun { get; set; } = string.Empty;
        [Index(92)]
        public decimal? NuHaita { get; set; } = 0;
        [Index(93)]
        public string DtRenkei { get; set; } = string.Empty;
        [Index(94)]
        public string CdRitchi { get; set; } = string.Empty;
        [Index(95)]
        public string CdTantosha { get; set; } = string.Empty;
        [Index(96)]
        public string DtKeiHoNyukin { get; set; } = string.Empty;
        [Index(97)]
        public decimal? MnKeiHoHoshoKingaku { get; set; } = 0;
        [Index(98)]
        public string NmKeiHoJishatanto { get; set; } = string.Empty;
        [Index(99)]
        public string NmKeiHoAiteTanto { get; set; } = string.Empty;
        [Index(100)]
        public string DtKaiHoNyukin { get; set; } = string.Empty;
        [Index(101)]
        public decimal? MnKaiHoHoshoKingaku { get; set; } = 0;
        [Index(102)]
        public string NmKaiHoJishatanto { get; set; } = string.Empty;
        [Index(103)]
        public string NmKaiHoAitetanto { get; set; } = string.Empty;
        [Index(104)]
        public string NmShukkalabelTenmei { get; set; } = string.Empty;
    }
}