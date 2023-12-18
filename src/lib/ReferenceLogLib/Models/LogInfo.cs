using Prism.Mvvm;
using System;

namespace ReferenceLogLib.Models
{
    public class LogInfo : BindableBase
    {
        private long _id;
        public long Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private bool _selected = false;
        public bool Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string DispSelected => Selected ? "●" : string.Empty;

        private string _startDate = string.Empty;
        public string StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        private string _endDate = string.Empty;
        public string EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        private string _shainCode = string.Empty;
        public string ShainCode
        {
            get => _shainCode;
            set => SetProperty(ref _shainCode, value);
        }
    }
}
