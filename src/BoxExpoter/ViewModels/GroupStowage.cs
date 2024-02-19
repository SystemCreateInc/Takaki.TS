using DbLib.Extensions;
using Prism.Mvvm;
using System;

namespace BoxExpoter.ViewModels
{
    public class GroupStowage : BindableBase
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

        private DateTime _dtDelivery = DateTime.Today;
        public DateTime DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

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

        public GroupStowage(int sendedCount, string dT_DELIVERY, string cD_DIST_GROUP, string nM_DIST_GROUP, int customerCount, int seatCount, int uncompletedCount, int completedCount, int overCount)
        {
            IsSended = (completedCount + uncompletedCount) == sendedCount;
            IsEnabled = uncompletedCount == 0;
            IsSelected = IsEnabled && !IsSended;
            DtDelivery = (DateTime)dT_DELIVERY.ParseNonSeparatedDate(DateTime.Today)!;
            CdDistGroup = cD_DIST_GROUP;
            NmDistGroup = nM_DIST_GROUP;
            CustomerCount = customerCount;
            SeatCount = seatCount;
            UncompletedCount = uncompletedCount;
            CompletedCount = completedCount;
            OverCount = overCount;
        }
    }
}