using DbLib.Extensions;

namespace WorkReport.Reports
{
    public class Group
    {
        public string DtDelivery { get; set; } = string.Empty;
        public string DispDtDelivery => DtDelivery.GetDate();
        public DateTime DtStart { get; set; }
        public string CdDistGroup { get; set; } = string.Empty;
        public string CdBlock { get; set; } = string.Empty;
        public DateTime DtEnd { get; set; }
        public int NmIdle { get; set; }
        public TimeSpan DispNmIdle => new TimeSpan(0, 0, NmIdle);

        // 全体の作業時間(終了時間 - 開始時間 - 休憩時間)
        public TimeSpan AllWorkTime => DtEnd - DtStart - DispNmIdle;
        public List<Detail> Details { get; set; } = new List<Detail>();

    }
}
