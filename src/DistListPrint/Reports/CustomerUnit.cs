namespace DistListPrint.Reports
{
    public class CustomerUnit
    {
        public string CdShukkaBatch { get; set; } = string.Empty;
        public string NmShukkaBatch { get; set; } = string.Empty;
        public string CdBlock { get; set; } = string.Empty;
        public string CdCourse { get; set; } = string.Empty;
        public int CdRoute { get; set; }
        public string CdTokuisaki { get; set; } = string.Empty;
        public string NmTokuisaki { get; set; } = string.Empty;
        public int TotalItemCount { get; set; }
        public int TotalNuOps { get; set; }
    }
}
