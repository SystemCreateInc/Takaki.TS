namespace DistGroup.Models
{
    // コース重複情報表示用クラス
    public class SameCourseInfo
    {
        public long IdDistGroup { get; internal set; }
        public string CdDistGroup { get; set; } = string.Empty;
        public string CdKyoten { get; set; } = string.Empty;
        public string Tekiyokaishi { get; set; } = string.Empty;
        public string TekiyoMuko { get; set; } = string.Empty;
        public string CdShukkaBatch { get; set; } = string.Empty;
        public IEnumerable<string> Courses { get; set; } = Enumerable.Empty<string>();
    }
}
