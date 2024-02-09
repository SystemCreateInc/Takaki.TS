using Prism.Mvvm;

namespace DistExpoter.ViewModels
{
    public class GroupDist : BindableBase
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isSended;
        public bool IsSended
        {
            get => _isSended;
            set => SetProperty(ref _isSended, value);
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }


        public string SendStatusText => IsSended ? "送信済" : "";

        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _nmDistGrouop = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGrouop;
            set => SetProperty(ref _nmDistGrouop, value);
        }

        private int _customerCount;
        public int CustomerCount
        {
            get => _customerCount;
            set => SetProperty(ref _customerCount, value);
        }

        private int _seatCount;
        public int SeatCount
        {
            get => _seatCount;
            set => SetProperty(ref _seatCount, value);
        }

        private int _completedCount;
        public int CompletedCount
        {
            get => _completedCount;
            set => SetProperty(ref _completedCount, value);
        }

        private int _uncompletedCount;
        public int UncompletedCount
        {
            get => _uncompletedCount;
            set => SetProperty(ref _uncompletedCount, value);
        }

        private int _overCount;
        public int OverCount
        {
            get => _overCount;
            set => SetProperty(ref _overCount, value);
        }

        private int _sendCount;
        public int SendCount
        {
            get => _sendCount;
            set => SetProperty(ref _sendCount, value);
        }
        private int _sendedCount;
        public int SendedCount
        {
            get => _sendedCount;
            set => SetProperty(ref _sendedCount, value);
        }





        public GroupDist(int sendCount, int sendedCount, string cD_DIST_GROUP, string nM_DIST_GROUP, int customerCount, int seatCount, int uncompletedCount, int completedCount, int overCount)
        {
            IsSended = sendedCount != 0;
            IsEnabled = true;
            IsSelected = IsEnabled && !IsSended;
            CdDistGroup = cD_DIST_GROUP;
            NmDistGroup = nM_DIST_GROUP;
            CustomerCount = customerCount;
            SeatCount = seatCount;
            UncompletedCount = uncompletedCount;
            CompletedCount = completedCount;
            OverCount = overCount;
            SendedCount = sendedCount;
            SendCount = sendCount;
        }
    }
}