using DbLib.Defs;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SeatThreshold.Models;

namespace SeatThreshold.ViewModels
{
    public class InputSeatThresholdViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }

        private readonly IDialogService _dialogService;

        // 参照日
        private DateTime _date;
        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        // 拠点コード
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set => SetProperty(ref _cdKyoten, value);
        }

        // 拠点名称
        private string _nmKyoten = string.Empty;
        public string NmKyoten
        {
            get => _nmKyoten;
            set => SetProperty(ref _nmKyoten, value);
        }

        // ブロック
        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set => SetProperty(ref _cdBlock, value);
        }

        // 種別
        private TdUnitType _tdUnitType = TdUnitType.TdCeiling;
        public TdUnitType TdUnitType
        {
            get => _tdUnitType;
            set => SetProperty(ref _tdUnitType, value);
        }

        // 適用開始日
        private DateTime _dtTekiyoKaishi;
        public DateTime DtTekiyoKaishi
        {
            get => _dtTekiyoKaishi;
            set => SetProperty(ref _dtTekiyoKaishi, value);
        }

        // 適用無効日
        private DateTime _dtTekiyoMuko;
        public DateTime DtTekiyoMuko
        {
            get => _dtTekiyoMuko;
            set => SetProperty(ref _dtTekiyoMuko, value);
        }

        // 登録日時
        private DateTime _dtTorokuNichiji;
        public DateTime DtTorokuNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtTorokuNichiji, value);
        }

        // 更新日時
        private DateTime _dtKoshinNichiji;
        public DateTime DtKoshinNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtKoshinNichiji, value);
        }

        // 更新者コード
        private string _cdShain = string.Empty;
        public string CdShain
        {
            get => _cdShain;
            set => SetProperty(ref _cdShain, value);
        }

        // 更新者名称
        private string _nmShain = string.Empty;
        public string NmShain
        {
            get => _nmShain;
            set => SetProperty(ref _nmShain, value);
        }

        // 表示器数
        private int _nuTdunitCnt;
        public int NuTdunitCnt
        {
            get => _nuTdunitCnt;
            set => SetProperty(ref _nuTdunitCnt, value);
        }

        // しきい値
        private int _nuThreshold;
        public int NuThreshold
        {
            get => _nuThreshold;
            set => SetProperty(ref _nuThreshold, value);
        }

        // 履歴表示リスト
        private List<Log> _logs = new List<Log>();
        public List<Log> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        public InputSeatThresholdViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdViewModel:Clear");
                // fixme:クリアボタン押下
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdViewModel:Register");
                // fixme:登録ボタン押下
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdViewModel:Back");
                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

            Refer = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdViewModel:Refer");
                // fixme:参照ボタン押下
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdViewModel:Release");
                // fixme:解除ボタン押下
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            InitDisplay();
        }

        private void InitDisplay()
        {
            // 項目欄確認
            Date = DateTime.Today;
            CdKyoten = "4201";
            NmKyoten = "広島工場製品出荷";
            CdBlock = "1";
            TdUnitType = TdUnitType.TdCeiling;
            DtTekiyoKaishi = new DateTime(2023, 10, 1);
            DtTekiyoMuko = new DateTime(2023, 12, 31);
            DtTorokuNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            DtKoshinNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            CdShain = "0033550";
            NmShain = "小田 賢行";
            NuTdunitCnt = 128;
            NuThreshold = 13;

            Logs = new List<Log>
            {
                new Log { Selected = false, DtTekiyoKaishi = "20230901", DtTekiyoMuko = "20231001", CdShain = "0033550", },
                new Log { Selected = true, DtTekiyoKaishi = "20231001", DtTekiyoMuko = "20231231", CdShain = "0033550", },
            };
        }
    }
}
