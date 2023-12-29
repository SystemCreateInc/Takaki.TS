using Prism.Mvvm;

namespace WorkReport.Reports
{
    public class Detail : BindableBase
    {
        private bool _isEven = false;
        public bool IsEven
        {
            get => _isEven;
            set => SetProperty(ref _isEven, value);
        }

        public string NmSyain { get; set; } = string.Empty;
        public int NmWorktime { get; set; }
        public TimeSpan DispNmWorktime => new TimeSpan(0, 0, NmWorktime);
        public int NmItemcnt { get; set; }
        public int Shopcnt { get; set; }
        public int NmDistcnt { get; set; }
        public int NmCheckcnt { get; set; }
        public int NmChecktime { get; set; }
        public TimeSpan DispNmChecktime => new TimeSpan(0, 0, NmChecktime);
    }
}
