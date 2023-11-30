using CsvHelper.Configuration.Attributes;

namespace ImportLib.Models
{
    public class StowageFileLine
    {
		[Index(0)]
        public string DtDelivery {get; set;} = string.Empty;
        [Index(1)]
        public string CdShukkaBatch { get; set; } = string.Empty;
        [Index(2)]
        public string CdKyoten { get; set; } = string.Empty;
        [Index(3)]
        public string CdHaishoBin { get; set; } = string.Empty;
        [Index(4)]
        public string CdCourse { get; set; } = string.Empty;
        [Index(5)]
        public int CdRoute { get; set; } = 0;
        [Index(6)]
        public string CdTokuisaki { get; set; } = string.Empty;
        [Index(7)]
        public short StBoxtype { get; set; } = 0;
        [Index(8)]
        public int NuOboxcnt { get; set; } = 0;
        [Index(9)]
        public string DtTorokuNichiji { get; set; } = string.Empty;
        [Index(10)]
        public string DtKoshinNichiji { get; set; } = string.Empty;
        [Index(11)]
        public string NmHenkosha { get; set; } = string.Empty;
    }
}