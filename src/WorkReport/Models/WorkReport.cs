using DbLib.Extensions;
using Prism.Mvvm;

namespace WorkReport.Models
{
    public class WorkReport : BindableBase
    {
        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }

        public string DispDtDelivery => DtDelivery.GetDate();

        private DateTime _dtStart;
        public DateTime DtStart
        {
            get => _dtStart;
            set => SetProperty(ref _dtStart, value);
        }

        private DateTime _dtEnd;
        public DateTime DtEnd
        {
            get => _dtEnd;
            set => SetProperty(ref _dtEnd, value);
        }

        private string _cdDistGroup = string.Empty;
        public string CdDistGroup
        {
            get => _cdDistGroup;
            set => SetProperty(ref _cdDistGroup, value);
        }

        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }

        // 休憩時間(秒)
        private int _nmIdle;
        public int NmIdle
        {
            get => _nmIdle;
            set => SetProperty(ref _nmIdle, value);
        }

        public TimeSpan DispNmIdle => new TimeSpan(0, 0, NmIdle);

        // 全体の作業時間(終了時間 - 開始時間 - 休憩時間)
        public TimeSpan AllWorkTime => DtEnd - DtStart - DispNmIdle;

        private string _nmSyain = string.Empty;
        public string NmSyain
        {
            get => _nmSyain;
            set => SetProperty(ref _nmSyain, value);
        }

        // 個人の作業時間(秒)
        private int _nmWorktime;
        public int NmWorktime
        {
            get => _nmWorktime;
            set => SetProperty(ref _nmWorktime, value);
        }

        public TimeSpan DispNmWorktime => new TimeSpan(0, 0, NmWorktime);

        private int _nmItemcnt;
        public int NmItemcnt
        {
            get => _nmItemcnt;
            set => SetProperty(ref _nmItemcnt, value);
        }

        private int _shopcnt;
        public int Shopcnt
        {
            get => _shopcnt;
            set => SetProperty(ref _shopcnt, value);
        }

        private int _nmDistcnt;
        public int NmDistcnt
        {
            get => _nmDistcnt;
            set => SetProperty(ref _nmDistcnt, value);
        }

        private int _nmCheckcnt;
        public int NmCheckcnt
        {
            get => _nmCheckcnt;
            set => SetProperty(ref _nmCheckcnt, value);
        }

        // 検品時間(秒)
        private int _nmChecktime;
        public int NmChecktime
        {
            get => _nmChecktime;
            set => SetProperty(ref _nmChecktime, value);
        }

        public TimeSpan DispNmChecktime => new TimeSpan(0, 0, NmChecktime);
    }
}
