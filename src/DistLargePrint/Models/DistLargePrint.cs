using Prism.Mvvm;

namespace DistLargePrint.Models
{
    public class DistLargePrint : BindableBase
    {
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

        private int _qtSet;
        public int QtSet
        {
            get => _qtSet;
            set => SetProperty(ref _qtSet, value);
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
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
