using CsvHelper.Configuration.Attributes;

namespace ImportLib.Models
{
    public class ShukkaBatchFileLine
    {
		[Index(0)]
        public string CdShukkaBatch { get; set; } = string.Empty;
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
        public string NmShukkaBatch { get; set; } = string.Empty;
        [Index(7)]
        public string CdShiwakeKyoten { get; set; } = string.Empty;
        [Index(8)]
        public string TmShiwakeKaishi { get; set; } = string.Empty;
        [Index(9)]
        public string TmShiwakeShuryo { get; set; } = string.Empty;
        [Index(10)]
        public string CdShukkaBatchGroup { get; set; } = string.Empty;
        [Index(11)]
        public string StOshiwakehyoShubetsu { get; set; } = string.Empty;
        [Index(12)]
        public string StChushiwakehyoShubetsu { get; set; } = string.Empty;
        [Index(13)]
        public string StKoshiwakehyoShubetsu { get; set; } = string.Empty;
        [Index(14)]
        public string FgTokuisakiShukei1 { get; set; } = string.Empty;
        [Index(15)]
        public string FgTokuisakiShukei2 { get; set; } = string.Empty;
        [Index(16)]
        public string FgTokuisakiShukei3 { get; set; } = string.Empty;
        [Index(17)]
        public string FgTokuisakiShukei4 { get; set; } = string.Empty;
        [Index(18)]
        public string FgTokuisakiShukei5 { get; set; } = string.Empty;
        [Index(19)]
        public string FgTokuisakiShukei6 { get; set; } = string.Empty;
        [Index(20)]
        public string FgHaisoubinBetsuShukeiUmu { get; set; } = string.Empty;
        [Index(21)]
        public string FgCourseBetsuShukeiUmu { get; set; } = string.Empty;
        [Index(22)]
        public string FgJuchujokyohyo { get; set; } = string.Empty;
        [Index(23)]
        public string FgSandHikitoriHyo { get; set; } = string.Empty;
        [Index(24)]
        public string StDpsShubetsu { get; set; } = string.Empty;
        [Index(25)]
        public decimal? NuShiwakeLeadTime { get; set; } = 0;
        [Index(26)]
        public string StLeadTimeSeigyo { get; set; } = string.Empty;
        [Index(27)]
        public string FgYobi1 { get; set; } = string.Empty;
        [Index(28)]
        public string FgYobi2 { get; set; } = string.Empty;
        [Index(29)]
        public string FgYobi3 { get; set; } = string.Empty;
        [Index(30)]
        public string FgYobi4 { get; set; } = string.Empty;
        [Index(31)]
        public string FgYobi5 { get; set; } = string.Empty;
        [Index(32)]
        public string StYobi1 { get; set; } = string.Empty;
        [Index(33)]
        public string StYobi2 { get; set; } = string.Empty;
        [Index(34)]
        public string StYobi3 { get; set; } = string.Empty;
        [Index(35)]
        public string StYobi4 { get; set; } = string.Empty;
        [Index(36)]
        public string StYobi5 { get; set; } = string.Empty;
        [Index(37)]
        public decimal? NuHaita { get; set; } = 0;
        [Index(38)]
        public string DtRenkei { get; set; } = string.Empty;
    }
}       
