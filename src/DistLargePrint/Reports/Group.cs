namespace DistLargePrint.Reports
{
    public class Group
    {
        public string CdHimban { get; set; } = string.Empty;
        public string CdJan { get; set; } = string.Empty;
        public string NmHinSeishikimei { get; set; } = string.Empty;
        public int NuBoxunit { get; set; }
        public int TotalPs { get; set; }
        public int TotalBoxPs { get; set; }
        public int TotalBaraPs { get; set; }
        public List<Detail> Details { get; set; } = new List<Detail>();
    }
}
