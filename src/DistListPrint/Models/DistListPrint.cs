using Prism.Mvvm;

namespace DistListPrint.Models
{
    public class DistListPrint : BindableBase
    {
        private long _idDist;
        public long IdDist
        {
            get => _idDist;
            set => SetProperty(ref _idDist, value);
        }

        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
        }

        private string _nmShukkaBatch = string.Empty;
        public string NmShukkaBatch
        {
            get => _nmShukkaBatch;
            set => SetProperty(ref _nmShukkaBatch, value);
        }

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private int _cdRoute;
        public int CdRoute
        {
            get => _cdRoute;
            set => SetProperty(ref _cdRoute, value);
        }

        private string _cdTokuisaki = string.Empty;
        public string CdTokuisaki
        {
            get => _cdTokuisaki;
            set => SetProperty(ref _cdTokuisaki, value);
        }

        private string _nmTokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nmTokuisaki;
            set => SetProperty(ref _nmTokuisaki, value);
        }

        private string _cdHimban = string.Empty;
        public string CdHimban
        {
            get => _cdHimban;
            set => SetProperty(ref _cdHimban, value);
        }

        private string _cdGtin13 = string.Empty;
        public string CdGtin13
        {
            get => _cdGtin13;
            set => SetProperty(ref _cdGtin13, value);
        }

        private string _nmHinSeishikimei = string.Empty;
        public string NmHinSeishikimei
        {
            get => _nmHinSeishikimei;
            set => SetProperty(ref _nmHinSeishikimei, value);
        }

        private int _nuBoxunit;
        public int NuBoxunit
        {
            get => _nuBoxunit;
            set => SetProperty(ref _nuBoxunit, value);
        }

        private int _boxOps;
        public int BoxOps
        {
            get => _boxOps;
            set => SetProperty(ref _boxOps, value);
        }

        private int _baraOps;
        public int BaraOps
        {
            get => _baraOps;
            set => SetProperty(ref _baraOps, value);
        }

        private int _nuOps;
        public int NuOps
        {
            get => _nuOps;
            set => SetProperty(ref _nuOps, value);
        }

        private int _boxRemainingPs;
        public int BoxRemainingPs
        {
            get => _boxRemainingPs;
            set => SetProperty(ref _boxRemainingPs, value);
        }

        private int _baraRemainingPs;
        public int BaraRemainingPs
        {
            get => _baraRemainingPs;
            set => SetProperty(ref _baraRemainingPs, value);
        }

        private int _totalRemainingPs;
        public int TotalRemainingPs
        {
            get => _totalRemainingPs;
            set => SetProperty(ref _totalRemainingPs, value);
        }

        private int _boxRps;
        public int BoxRps
        {
            get => _boxRps;
            set => SetProperty(ref _boxRps, value);
        }

        private int _baraRps;
        public int BaraRps
        {
            get => _baraRps;
            set => SetProperty(ref _baraRps, value);
        }

        private int _nuDrps;
        public int NuDrps
        {
            get => _nuDrps;
            set => SetProperty(ref _nuDrps, value);
        }

        private DateTime? _dtWorkdtDist;
        public DateTime? DtWorkdtDist
        {
            get => _dtWorkdtDist;
            set => SetProperty(ref _dtWorkdtDist, value);
        }

        private string? _nmShainDist;
        public string? NmShainDist
        {
            get => _nmShainDist;
            set => SetProperty(ref _nmShainDist, value);
        }
    }
}
