using Customer.Loader;
using Customer.Models;
using DbLib.Extensions;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Security.Cryptography.Xml;
using WindowLib.Utils;

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
        private DateTime _referenceDate;
        public DateTime ReferenceDate
        {
            get => _referenceDate;
            set => SetProperty(ref _referenceDate, value);
        }

        // 拠点コード
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set
            {
                SetProperty(ref _cdKyoten, value);
                NmKyoten = KyotenLoader.GetName(CdKyoten);
                _isChange = true;
            }
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
            set
            {
                SetProperty(ref _cdSumTokuisaki, value);
                NmSumTokuisaki = CustomerLoader.GetName(CdSumTokuisaki);
                _isChange = true;
            }
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
            set
            {
                SetProperty(ref _dtTekiyoKaishi, value);
                if (!IsAdd && !_isInitializeing && IsDateRelease)
                {
                    RegistCustomer();
                }
            }
        }

        // 適用無効日
        private DateTime _dtTekiyoMuko;
        public DateTime DtTekiyoMuko
        {
            get => _dtTekiyoMuko;
            set
            {
                SetProperty(ref _dtTekiyoMuko, value);
                if (!IsAdd && !_isInitializeing && IsDateRelease)
                {
                    RegistCustomer();
                }
            }
        }

        // 登録日時
        private DateTime? _dtTorokuNichiji;
        public DateTime? DtTorokuNichiji
        {
            get => _dtTorokuNichiji;
            set => SetProperty(ref _dtTorokuNichiji, value);
        }

        // 更新日時
        private DateTime? _dtKoshinNichiji;
        public DateTime? DtKoshinNichiji
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
        private ObservableCollection<ChildCustomer> _childCustomer = new ObservableCollection<ChildCustomer>();
        public ObservableCollection<ChildCustomer> ChildCustomers
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

        private SumCustomer _currentCustomer = new SumCustomer();

        private bool _isDateRelease = false;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }

        private bool _isAdd = false;
        public bool IsAdd
        {
            get => _isAdd;
            set => SetProperty(ref _isAdd, value);
        }

        private bool _isInitializeing = false;

        private ShainInfo _shainInfo = new ShainInfo();

        private bool _isChange = false;

        public InputCustomerViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

			Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Clear");
                ClearInfo(IsAdd);
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Register");
                if (RegistCustomer())
                {
                    regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
                }
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Back");
                if (ConfirmationExit())
                {
                    regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
                }
            });

            Refer = new DelegateCommand(() =>
            {
                if (!Logs.Any())
                {
                    return;
                }

                Syslog.Debug("InputCustomerViewModel:Refer");
                ClearInfo(IsAdd);
                SetReferenceCustomer();
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Release");
                IsDateRelease = true;
            });
        }

        private bool ConfirmationExit()
        {
            if (_isChange)
            {
                if(MessageDialog.Show(_dialogService, "変更された情報が登録されていません。\n入力画面に戻りますか？", "変更確認", ButtonMask.Yes | ButtonMask.No) != ButtonResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        // 参照
        private void SetReferenceCustomer()
        {
            var customer = CustomerLoader.GetFromReferenceDate(_currentCustomer.CdKyoten, _currentCustomer.CdSumTokuisaki, ReferenceDate);

            if (customer is not null)
            {
                DtTorokuNichiji = customer.CreatedAt;
                DtKoshinNichiji = customer.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                ChildCustomers = new ObservableCollection<ChildCustomer>(customer.ChildCustomers);
                DtTekiyoKaishi = DateTime.Parse(customer.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(customer.TekiyoMuko.GetDate());
            }

            SelectChangeLogs(customer?.Tekiyokaishi ?? string.Empty);

            _isChange = false;
        }

        // 選択状態更新
        private void SelectChangeLogs(string tekiyokaishi)
        {
            Logs = Logs.Select(x => new Log
            {
                Selected = x.DtTekiyoKaishi == tekiyokaishi,
                CdShain = x.CdShain,
                DtTekiyoKaishi = x.DtTekiyoKaishi,
                DtTekiyoMuko = x.DtTekiyoMuko,
            }).ToList();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ChildCustomers.CollectionChanged -= ChildCustomers_CollectionChanged;
            _isChange = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _currentCustomer = (SumCustomer)navigationContext.Parameters["CurrentCustomer"];
            _shainInfo = (ShainInfo)navigationContext.Parameters["ShainInfo"];

            IsAdd = _currentCustomer is null;
            InitDisplay();

            ChildCustomers.CollectionChanged += ChildCustomers_CollectionChanged;
        }

        private void ChildCustomers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isChange = true;
        }

        private void InitDisplay()
        {
            _isInitializeing = true;

            ClearInfo(true);

            if (!IsAdd)
            {
                CdKyoten = _currentCustomer.CdKyoten;
                CdSumTokuisaki = _currentCustomer.CdSumTokuisaki;
                Logs = LogLoader.Get(_currentCustomer.CdKyoten, _currentCustomer.CdSumTokuisaki).ToList();
                // 参照日初期値で履歴から検索
                SetReferenceCustomer();
            }
            else
            {                
                Logs = new List<Log>();
            }

            _isInitializeing = false;
        }

        private void ClearInfo(bool isAll)
        {
            IsDateRelease = false;

            if (isAll)
            {
                ReferenceDate = DateTime.Today;
                CdKyoten = string.Empty;
                NmKyoten = string.Empty;
                CdSumTokuisaki = string.Empty;
                NmSumTokuisaki = string.Empty;
            }

            DtTekiyoKaishi = DateTime.Today;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdShain = string.Empty;
            NmShain = string.Empty;
            ChildCustomers = new ObservableCollection<ChildCustomer>();

            _isChange = false;
        }

        private bool RegistCustomer()
        {
            if (!ValidInput())
            {
                return false;
            }

            try
            {
                var targetCustomer = new SumCustomer
                {
                    CdKyoten = CdKyoten,
                    CdSumTokuisaki = CdSumTokuisaki,
                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                    ChildCustomers = new List<ChildCustomer>(ChildCustomers),
                };

                var existCustomer = CustomerLoader.GetFromKey(CdKyoten, CdSumTokuisaki, DtTekiyoKaishi.ToString("yyyyMMdd"));

                if (IsAdd)
                {
                    if (existCustomer is not null)
                    {
                        MessageDialog.Show(_dialogService, $"拠点[{CdKyoten}],集約得意先[{CdSumTokuisaki}],適用開始日[{DtTekiyoKaishi}]\n同一組み合わせのデータが登録済みです", "入力エラー");
                    }

                    CustomerManager.Regist(targetCustomer, _shainInfo);
                }
                else if(existCustomer is not null)
                {
                    targetCustomer.SumTokuisakiId = existCustomer.SumTokuisakiId;
                    CustomerManager.Update(targetCustomer, _shainInfo);
                }
                else
                {
                    CustomerManager.Regist(targetCustomer, _shainInfo);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                return false;
            }
        }

        private bool ValidInput()
        {
            bool isValid = true;

            if (CdKyoten.IsNullOrEmpty() ||
                CdSumTokuisaki.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点コード、集約得意先コードを入力してください。", "入力エラー");
                return false;
            }

            if (!ChildCustomers.Any())
            {
                MessageDialog.Show(_dialogService, "子得意先を追加して下さい", "入力エラー");
                return false;
            }

            return isValid;
        }

        // 摘要期間チェック
        private bool ValidSummaryDate()
        {
            if(DtTekiyoKaishi < DateTime.Today)
            {
                MessageDialog.Show(_dialogService, "摘要開始日が過去日です", "入力エラー");
                return false;
            }

            if(DtTekiyoKaishi > DtTekiyoMuko)
            {
                MessageDialog.Show(_dialogService, "摘要開始日より摘要無効日が過去日です", "入力エラー");
                return false;
            }

            // TODO:摘要期間重複チェック追加

            return true;
        }
    }
}
