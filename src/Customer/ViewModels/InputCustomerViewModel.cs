using Customer.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace Customer.ViewModels
{
    public class InputCustomerViewModel : BindableBase, INavigationAware
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

        // 集約得意先コード
        private string _cdSumTokuisaki = string.Empty;
        public string CdSumTokuisaki
        {
            get => _cdSumTokuisaki;
            set => SetProperty(ref _cdSumTokuisaki, value);
        }

        // 集約得意先名称
        private string _nmSumTokuisaki = string.Empty;
        public string NmSumTokuisaki
        {
            get => _nmSumTokuisaki;
            set => SetProperty(ref _nmSumTokuisaki, value);
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

        // 子得意先リスト
        private List<ChildCustomer> _childCustomer = new List<ChildCustomer>();
        public List<ChildCustomer> ChildCustomers
        {
            get => _childCustomer;
            set => SetProperty(ref _childCustomer, value);
        }

        // 履歴表示リスト
        private List<Log> _logs = new List<Log>();
        public List<Log> Logs
        {
            get => _logs;
            set => SetProperty(ref _logs, value);
        }

        public InputCustomerViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Clear");
                // fixme:クリアボタン押下
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Register");
                // fixme:登録ボタン押下
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Back");
                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

            Refer = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Refer");
                // fixme:参照ボタン押下
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Release");
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
            CdSumTokuisaki = "200061";
            NmSumTokuisaki = "得意先名称";
            DtTekiyoKaishi = new DateTime(2023, 10, 1);
            DtTekiyoMuko = new DateTime(2023, 12, 31);
            DtTorokuNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            DtKoshinNichiji = new DateTime(2023, 10, 11, 12, 34, 56);
            CdShain = "0033550";
            NmShain = "小田　賢行";
            ChildCustomers = new List<ChildCustomer>
            {
                new ChildCustomer { CdTokuisakiChild = "2000062", NmTokuisaki = "得意先名称" },
            };

            Logs = new List<Log>
            {
                new Log { Selected = false, DtTekiyoKaishi = "20230901", DtTekiyoMuko = "20231001", CdShain = "0033550" },
                new Log { Selected = true, DtTekiyoKaishi = "20231001", DtTekiyoMuko = "20231231", CdShain = "0033550" },
            };
        }
    }
}
