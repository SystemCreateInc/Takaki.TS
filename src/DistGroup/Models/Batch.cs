using Prism.Mvvm;

namespace DistGroup.Models
{
    public class Batch : BindableBase
    {
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

        private string _cdLargeGroup = string.Empty;
        public string CdLargeGroup
        {
            get => _cdLargeGroup;
            set => SetProperty(ref _cdLargeGroup, value);
        }

        private string _cdLargeGroupName = string.Empty;
        public string CdLargeGroupName
        {
            get => _cdLargeGroupName;
            set => SetProperty(ref _cdLargeGroupName, value);
        }
    }
}
