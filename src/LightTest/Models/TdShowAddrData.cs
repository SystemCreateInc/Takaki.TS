using DbLib.Defs;
using DbLib.Extensions;
using TdDpsLib.Defs;
using TdDpsLib.Models;

namespace LightTest.Models
{
    public class TdShowAddrData : TdAddrData
    {
        public TdShowAddrData(TdAddrData tad)
        {
            TdUnitCode = tad.TdUnitCode;
            TdUnitAddrCode = tad.TdUnitAddrCode;
            TddGroup = tad.TddGroup;
            TddAddr = tad.TddAddr;
            TddButton = tad.TddButton;
            Stno = tad.Stno;
            TextLen = tad.TextLen;

            TdUnitPortType = tad.TdUnitPortType;
            TdUnitPortCode = tad.TdUnitPortCode;
            TdPortCom = tad.TdPortCom;

            // Areas = tad.Areas;
        }

        private bool _select = true;
        public bool Select
        {
            get => _select;
            set => SetProperty(ref _select, value);
        }

        private string[] _btnStatus = { "OFF", "OFF", "OFF", "OFF", "OFF" };
        public string[] BtnStatus
        {
            get => _btnStatus;
            set => SetProperty(ref _btnStatus, value);
        }

        /// <summary>
        /// ボタンステータス変更+通知
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="status"></param>
        public void SetButton(int btn, string status)
        {
            BtnStatus[btn - 1] = status;

            RaisePropertyChanged(nameof(BtnStatus));
        }

        private string _dstatusName = "";

        public string DStatusName
        {
            get => _dstatusName;
            set => SetProperty(ref _dstatusName, value);
        }

        public void SetDStatusName(TdDisplayUnitStatus status)
        {
            DStatusName = EnumExtensions.GetDescription(status);
        }

        private string _batteryInfoName = "";

        public string BatteryInfoName
        {
            get => _batteryInfoName;
            set => SetProperty(ref _batteryInfoName, value);
        }

        private string _errsatusname = "";

        public string ErrStatusName
        {
            get => _errsatusname;
            set => SetProperty(ref _errsatusname, value);
        }

        private string _lighttesterrsatusname = "";

        public string LightTestErrStatusName
        {
            get => _lighttesterrsatusname;
            set => SetProperty(ref _lighttesterrsatusname, value);
        }

        public void SetBatteryInfoName(TdDisplayUnitStatus status)
        {
            if (status == TdDisplayUnitStatus.High
                || status == TdDisplayUnitStatus.Low
                || status == TdDisplayUnitStatus.Mid
                || status == TdDisplayUnitStatus.Ready)
            {
                BatteryInfoName = EnumExtensions.GetDescription(status);
            }
        }

        // 表示器がエラーか？
        public bool IsTdErr(string text)
        {
            LightTestErrStatusName = "";

            switch (text)
            {
                case "TIM":
                    // LightTestErrStatusName = EnumExtensions.GetDescription(TdDisplayUnitStatus.Error);
                    LightTestErrStatusName = "異常";
                    break;
                case "NAK":
                    // LightTestErrStatusName = EnumExtensions.GetDescription(TdDisplayUnitStatus.Nak);
                    LightTestErrStatusName = "異常";
                    break;
                case "OFF":
                    LightTestErrStatusName = EnumExtensions.GetDescription(TdDisplayUnitStatus.Off);
                    break;
                case "MID":
                    LightTestErrStatusName = EnumExtensions.GetDescription(TdDisplayUnitStatus.Mid);
                    break;
                case "LOW":
                    LightTestErrStatusName = EnumExtensions.GetDescription(TdDisplayUnitStatus.Low);
                    break;
                default:
                    break;
            }

            return LightTestErrStatusName == "" ? false : true;
        }
    }

    public class TdShowPortData : TdPortData
    {
        public TdShowPortData(TdPortData p)
        {
            Stno = p.Stno;
            TdUnitPortCode = p.TdUnitPortCode;
            TdPortCom = p.TdPortCom;
            Baudrate = p.Baudrate;
            TdUnitPortType = p.TdUnitPortType;
            TdUnitPortTypeName = p.TdUnitPortTypeName;
            PortStatus = p.PortStatus;
        }

        private string _portstatusname = "";
        public string PortStatusName
        {
            get => _portstatusname;
            set => SetProperty(ref _portstatusname, value);
        }

        public void SetPortStatusName(TdPortStatus status)
        {
            PortStatusName = EnumExtensions.GetDescription(status);
        }
    }
}
