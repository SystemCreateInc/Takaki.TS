using Prism.Mvvm;

namespace DistLargePrint.Reports
{
    public class Detail : BindableBase
    {
        private bool _isEven = false;
        public bool IsEven
        {
            get => _isEven;
            set => SetProperty(ref _isEven, value);
        }

        public string CdBlock { get; set; } = string.Empty;
        public int BoxOps { get; set; }
        public int BaraOps { get; set; }
        public int NuLops { get; set; }
        public int BoxRemainingPs { get; set; }
        public int BaraRemainingPs { get; set; }
        public int TotalRemainingPs { get; set; }
        public int BoxRps { get; set; }
        public int BaraRps { get; set; }
        public int NuLrps { get; set; }
        public DateTime? DtWorkdtLarge { get; set; }
        public string? NMShainLarge { get; set; }
    }
}
