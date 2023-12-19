using Customer.Loader;
using Customer.Models;
using DbLib.Extensions;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using System.Collections.ObjectModel;
using TakakiLib.Models;
using WindowLib.Utils;

namespace Customer.ViewModels
{
    public class InputCustomerViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }

        private readonly IDialogService _dialogService;

        public bool CanCloseDialog() => ConfirmationExit();

        private DateTime _referenceDate;
        public DateTime ReferenceDate
        {
            get => _referenceDate;
            set
            {
                SetProperty(ref _referenceDate, value);                
                ReloadTekiyoName();
            }
        }

        // 拠点コード
        private string _cdKyoten = string.Empty;
        public string CdKyoten
        {
            get => _cdKyoten;
            set
            {
                SetProperty(ref _cdKyoten, value);
                NmKyoten = NameLoader.GetKyoten(CdKyoten);
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
                NmSumTokuisaki = NameLoader.GetTokuisaki(CdSumTokuisaki);
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
                _isChange = true;
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
                _isChange = true;
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

        private ShainInfo _shainInfo = new ShainInfo();

        private bool _isChange = false;

        public event Action<IDialogResult>? RequestClose;

        public ReferenceLog ReferenceLog { get; set; } = new ReferenceLog();

        public string Title => "集約得意先情報入力";

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
                if (Regist())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputCustomerViewModel:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                if (!ReferenceLog.LogInfos.Any() || IsAdd)
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

        public void OnDialogClosed()
        {
            ChildCustomers.CollectionChanged -= ChildCustomers_CollectionChanged;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _currentCustomer = parameters.GetValue<SumCustomer>("CurrentCustomer");
            _shainInfo = parameters.GetValue<ShainInfo>("ShainInfo");

            IsAdd = _currentCustomer is null;
            InitDisplay();

            ChildCustomers.CollectionChanged += ChildCustomers_CollectionChanged;
        }

        private bool ConfirmationExit()
        {
            if (_isChange)
            {
                if (MessageDialog.Show(_dialogService, "変更された情報が登録されていません。\n一覧画面に戻りますか？", "変更確認", ButtonMask.Yes | ButtonMask.No) != ButtonResult.Yes)
                {
                    return false;
                }
            }

            return true;
        }

        // 参照
        private void SetReferenceCustomer()
        {
            var tekiyoDate = ReferenceLog.GetStartDateInRange(ReferenceDate.ToString("yyyyMMdd"));
            var customer = CustomerLoader.GetFromKey(_currentCustomer.CdKyoten, _currentCustomer.CdSumTokuisaki, tekiyoDate);

            if (customer is not null)
            {
                DtTorokuNichiji = customer.CreatedAt;
                DtKoshinNichiji = customer.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                DtTekiyoKaishi = DateTime.Parse(customer.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(customer.TekiyoMuko.GetDate());
                
                ChildCustomers = new ObservableCollection<ChildCustomer>(customer.ChildCustomers);
            }
            _isChange = false;
        }

        private void ChildCustomers_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _isChange = true;
        }

        private void InitDisplay()
        {
            ClearInfo(true);

            if (!IsAdd)
            {
                CdKyoten = _currentCustomer.CdKyoten;
                CdSumTokuisaki = _currentCustomer.CdSumTokuisaki;
                ReferenceLog.LogInfos = LogLoader.Get(_currentCustomer.CdKyoten, _currentCustomer.CdSumTokuisaki).ToList();

                // 参照日初期値で履歴から検索
                SetReferenceCustomer();
            }
            else
            {
                ReferenceLog.LogInfos.Clear();
            }
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

        private bool Regist()
        {
            try
            {
                if (!ValidateInput())
                {
                    return false;
                }

                var targetCustomer = new SumCustomer
                {
                    CdKyoten = CdKyoten.PadLeft(4, '0'),
                    CdSumTokuisaki = CdSumTokuisaki.PadLeft(6, '0'),
                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                    ChildCustomers = new List<ChildCustomer>(ChildCustomers.Where(x => !x.CdTokuisakiChild.IsNullOrEmpty())),
                };

                var existCustomer = CustomerLoader.GetFromKey(targetCustomer.CdKyoten, targetCustomer.CdSumTokuisaki, targetCustomer.Tekiyokaishi);
                var isExist = existCustomer is not null;

                if (!ValidateSummaryDate(isExist))
                {
                    return false;
                }

                if (!IsDuplicationCustomer(existCustomer?.SumTokuisakiId))
                {
                    return false;
                }

                if (IsAdd)
                {
                    if (isExist)
                    {
                        MessageDialog.Show(_dialogService, $"拠点[{CdKyoten}],集約得意先[{CdSumTokuisaki}],適用開始日[{DtTekiyoKaishi}]\n同一組み合わせのデータが登録済みです", "入力エラー");
                    }

                    CustomerManager.Regist(targetCustomer, _shainInfo);
                }
                else if (isExist)
                {
                    targetCustomer.SumTokuisakiId = existCustomer!.SumTokuisakiId;
                    CustomerManager.Update(targetCustomer, _shainInfo);
                }
                else
                {
                    CustomerManager.Regist(targetCustomer, _shainInfo);
                }

                _isChange = false;
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                return false;
            }
        }

        private bool ValidateInput()
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

            if (ChildCustomers.Where(x => !x.CdTokuisakiChild.IsNullOrEmpty()).Count() > 10)
            {
                MessageDialog.Show(_dialogService, "子得意先は10件まで登録可能です", "入力エラー");
                return false;
            }

            // 得意先名取得不可
            if (NmSumTokuisaki.IsNullOrEmpty()
                || ChildCustomers.Any(x => !x.CdTokuisakiChild.IsNullOrEmpty() && x.NmTokuisaki.IsNullOrEmpty()))
            {
                MessageDialog.Show(_dialogService, "得意先名が取得出来ていない得意先コードがあります", "入力エラー");
                return false;
            }

            return isValid;
        }

        // 適用期間チェック
        private bool ValidateSummaryDate(bool isUpdate)
        {
            try
            {
                ReferenceLog.LogInfos = LogLoader.Get(CdKyoten.PadLeft(4, '0'), CdSumTokuisaki.PadLeft(6, '0')).ToList();
                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, isUpdate);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "入力エラー");
                return false;
            }            
        }

        // 
        private bool IsDuplicationCustomer(long? sumTokuisakiId)
        {
            if(ChildCustomers.Any(x => x.CdTokuisakiChild == CdSumTokuisaki))
            {
                MessageDialog.Show(_dialogService, $"親得意先と子得意先に同一の得意先が存在します", "入力エラー");
                return false;
            }

            if(ChildCustomers.GroupBy(x => x.CdTokuisakiChild).Any(x => x.Count() > 1))
            {
                MessageDialog.Show(_dialogService, $"子得意先に同一の得意先が存在します", "入力エラー");
                return false;
            }

            // 親と同一得意先　更新時：自ID以外
            var sameCustomer = CustomerLoader.GetSameCustomer(new List<string> { CdSumTokuisaki }, 
                DtTekiyoKaishi.ToString("yyyyMMdd"), DtTekiyoMuko.ToString("yyyyMMdd"),
                sumTokuisakiId);

            if (sameCustomer is not null)
            {
                MessageDialog.Show(_dialogService, 
                    $"親得意先が、他の集約得意先に登録されています\n\n" + GetSameCustomerMessage(sameCustomer), "入力エラー");
                return false;
            }

            // 子と同一得意先　更新時：自ID以外
            sameCustomer = CustomerLoader.GetSameCustomer(ChildCustomers.Select(x => x.CdTokuisakiChild),
                DtTekiyoKaishi.ToString("yyyyMMdd"), DtTekiyoMuko.ToString("yyyyMMdd"),
                sumTokuisakiId);

            if (sameCustomer is not null)
            {
                MessageDialog.Show(_dialogService,
                    $"子得意先が、他の集約得意先に登録されています\n\n" + GetSameCustomerMessage(sameCustomer), "入力エラー");
                return false;
            }

            return true;
        }

        private string GetSameCustomerMessage(SumCustomer sameCustomer)
        {
            return $"拠点[{sameCustomer.CdKyoten}] 集約得意先[{sameCustomer.CdSumTokuisaki}]\n" +
                   $"適用開始-適用無効[{sameCustomer.Tekiyokaishi}-{sameCustomer.TekiyoMuko}]";
        }

        // 適用名称再取得
        private void ReloadTekiyoName()
        {
            // 静的参照日更新
            TekiyoDate.ReferenceDate = ReferenceDate.ToString("yyyyMMdd");

            NmSumTokuisaki = NameLoader.GetTokuisaki(CdSumTokuisaki);

            NmKyoten = NameLoader.GetKyoten(CdKyoten);

            ChildCustomers = new ObservableCollection<ChildCustomer>(ChildCustomers.Select(x => new ChildCustomer
            {
                CdTokuisakiChild = x.CdTokuisakiChild,
            }));
        }
    }
}
