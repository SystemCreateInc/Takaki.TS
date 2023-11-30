using DbLib.Defs;
using DbLib.Extensions;
using Prism.Mvvm;

namespace DistGroup.Models
{
    public class DistGroup : BindableBase
    {
        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _nmDistGroup = string.Empty;
        public string NmDistGroup
        {
            get => _nmDistGroup;
            set => SetProperty(ref _nmDistGroup, value);
        }

        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set => SetProperty(ref _cdKyoten, value);
        }

        private string _nmKyoten = string.Empty;
        public string NmKyoten
        {
            get => _nmKyoten;
            set => SetProperty(ref _nmKyoten, value);
        }

        private BinSumType _cdBinSum;
        public BinSumType CdBinSum
        {
            get => _cdBinSum;
            set => SetProperty(ref _cdBinSum, value);
        }

        public string DispBinSum => EnumExtensions.GetDescription(CdBinSum);

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
