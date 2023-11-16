using Prism.Mvvm;
using TdDpsLib.Defs;
using DbLib.Defs;
using DbLib.Extensions;

namespace TdDpsLib.Models
{
    // 表示器ポート情報クラス
    public class TdPortData : BindableBase
    {
        // 有線分岐No
        public int Stno { get; set; }
        // ポート
        public short TdUnitPortCode { get; set; }
        public string? TdPortCom { get; set; }
        // ボーレート
        public int Baudrate { get; set; }
        public TdControllerType TdUnitPortType { get; set; }
        public string? TdUnitPortTypeName { get; set; }

        // ポート状態
        private TdPortStatus _portstatus = TdPortStatus.Ready;

        public TdPortStatus PortStatus
        {
            get => _portstatus;
            set
            {
                if (_portstatus == value)
                    return;

                _portstatus = value;

//                RaisePropertyChanged(nameof(PortStatus));
            }
        }
        public string GetPortStatusName
        {
            get
            {
                return EnumExtensions.GetDescription(PortStatus);
            }
        }
    }
}
