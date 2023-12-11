using DbLib.Defs;
using DbLib.Extensions;
using Prism.Mvvm;

namespace SeatThreshold.Models
{
    public class ThresholdInfo : BindableBase
    {
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set => SetProperty(ref _cdKyoten, value);
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }

        private TdUnitType _tdUnitType;
        public TdUnitType TdUnitType
        {
            get => _tdUnitType;
            set => SetProperty(ref _tdUnitType, value);
        }

        public string DispTdUnitType => EnumExtensions.GetDescription(TdUnitType);

        private int _nuTdunitCnt;
        public int NuTdunitCnt
        {
            get => _nuTdunitCnt;
            set => SetProperty(ref _nuTdunitCnt, value);
        }

        private int _nuThreshold;
        public int NuThreshold
        {
            get => _nuThreshold;
            set => SetProperty(ref _nuThreshold, value);
        }

        public long BlockId { get; internal set; }
        public string Tekiyokaishi { get; set; } = string.Empty;
        public string TekiyoMuko { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
