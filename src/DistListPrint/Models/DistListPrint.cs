using Prism.Mvvm;

namespace DistListPrint.Models
{
    public class DistListPrint : BindableBase
    {
        private string _cdShukkaBatch = string.Empty;
        public string CdShukkaBatch
        {
            get => _cdShukkaBatch;
            set => SetProperty(ref _cdShukkaBatch, value);
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

        private string _cdJan = string.Empty;
        public string CdJan
        {
            get => _cdJan;
            set => SetProperty(ref _cdJan, value);
        }

        private string _nmHinSeishikimei = string.Empty;
        public string NmHinSeishikimei
        {
            get => _nmHinSeishikimei;
            set => SetProperty(ref _nmHinSeishikimei, value);
        }

        private int _qtUmpanYokiHakoIrisu;
        public int QtUmpanYokiHakoIrisu
        {
            get => _qtUmpanYokiHakoIrisu;
            set => SetProperty(ref _qtUmpanYokiHakoIrisu, value);
        }

        private int _obox;
        public int Obox
        {
            get => _obox;
            set => SetProperty(ref _obox, value);
        }

        private int _oBara;
        public int OBara
        {
            get => _oBara;
            set => SetProperty(ref _oBara, value);
        }

        private int _totalOps;
        public int TotalOps
        {
            get => _totalOps;
            set => SetProperty(ref _totalOps, value);
        }

        private int _remainingBox;
        public int RemainingBox
        {
            get => _remainingBox;
            set => SetProperty(ref _remainingBox, value);
        }

        private int _remainingBara;
        public int RemainingBara
        {
            get => _remainingBara;
            set => SetProperty(ref _remainingBara, value);
        }

        private int _totalRemainingps;
        public int TotalRemainingps
        {
            get => _totalRemainingps;
            set => SetProperty(ref _totalRemainingps, value);
        }

        private int _rbox;
        public int Rbox
        {
            get => _rbox;
            set => SetProperty(ref _rbox, value);
        }

        private int _rBara;
        public int RBara
        {
            get => _rBara;
            set => SetProperty(ref _rBara, value);
        }

        private int _totalRps;
        public int TotalRps
        {
            get => _totalRps;
            set => SetProperty(ref _totalRps, value);
        }

        private DateTime? _dtTorokuNichiji;
        public DateTime? DtTorokuNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtTorokuNichiji, value);
        }

        private string _nmHenkosha = string.Empty;
        public string NmHenkosha
        {
            get => _nmHenkosha;
            set => SetProperty(ref _nmHenkosha, value);
        }
    }
}
