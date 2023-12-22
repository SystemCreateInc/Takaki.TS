using Customer.Models;
using DbLib.Defs;
using DbLib.Extensions;
using LogLib;
using Microsoft.IdentityModel.Tokens;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using SeatThreshold.Loader;
using SeatThreshold.Models;
using System.Collections.ObjectModel;
using TakakiLib.Models;
using WindowLib.Utils;

namespace SeatThreshold.ViewModels
{
    public class InputSeatThresholdDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand Clear { get; }
        public DelegateCommand Register { get; }
        public DelegateCommand Back { get; }
        public DelegateCommand Refer { get; }
        public DelegateCommand Release { get; }

        public string Title => "座席しきい値情報入力";
        public event Action<IDialogResult>? RequestClose;
        private ThresholdInfo _seatThreshold = new ThresholdInfo();

        // 参照日
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

        // ブロック
        private string _cdBlock = string.Empty;
        public string CdBlock
        {
            get => _cdBlock;
            set
            {
                SetProperty(ref _cdBlock, value);
                _isChange = true;
            }
        }

        // 種別
        private TdUnitType _tdUnitType = TdUnitType.TdCeiling;
        public TdUnitType TdUnitType
        {
            get => _tdUnitType;
            set
            {
                SetProperty(ref _tdUnitType, value);
                _isChange = true;
            }
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
            get => _dtKoshinNichiji;
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
        private string _nuTdunitCnt = string.Empty;
        public string NuTdunitCnt
        {
            get => _nuTdunitCnt;
            set
            {
                SetProperty(ref _nuTdunitCnt, value);
                _isChange = true;
            }
        }

        // しきい値
        private string _nuThreshold = string.Empty;
        public string NuThreshold
        {
            get => _nuThreshold;
            set
            {
                SetProperty(ref _nuThreshold, value);
                _isChange = true;
            }
        }

        private bool _isAdd = false;
        public bool IsAdd
        {
            get => _isAdd;
            set => SetProperty(ref _isAdd, value);
        }

        private bool _isDateRelease = false;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }

        private ShainInfo _shainInfo = new ShainInfo();

        public ReferenceLog ReferenceLog { get; set; } = new ReferenceLog();

        private bool _isChange = false;

        private readonly IDialogService _dialogService;

        public InputSeatThresholdDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Clear = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdDlgViewModel:Clear");
                ClearInfo(IsAdd);
            });

            Register = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdDlgViewModel:Register");
                if (Regist())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdDlgViewModel:Back");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });

            Refer = new DelegateCommand(() =>
            {
                if (!ReferenceLog.LogInfos.Any() || IsAdd)
                {
                    return;
                }

                Syslog.Debug("InputSeatThresholdDlgViewModel:Refer");
                ClearInfo(IsAdd);
                SetReferenceInfo();
            });

            Release = new DelegateCommand(() =>
            {
                Syslog.Debug("InputSeatThresholdDlgViewModel:Release");
                IsDateRelease = true;
            });
        }

        public bool CanCloseDialog() => ConfirmationExit();

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _seatThreshold = parameters.GetValue<ThresholdInfo>("SeatThreshold");
            _shainInfo = parameters.GetValue<ShainInfo>("ShainInfo");
            IsAdd = _seatThreshold.CdKyoten.IsNullOrEmpty();
            InitDialog();
        }

        private void InitDialog()
        {
            ClearInfo(true);

            if (!IsAdd)
            {
                CdKyoten = _seatThreshold.CdKyoten;
                CdBlock = _seatThreshold.CdBlock;
                ReferenceLog.LogInfos = LogLoader.Get(_seatThreshold.CdKyoten, _seatThreshold.CdBlock).ToList();

                SetReferenceInfo();
            }
            else
            {
                ReferenceLog.LogInfos.Clear();
            }
        }

        private void ClearInfo(bool isAll)
        {
            if (isAll)
            {
                ReferenceDate = DateTime.Today;
                CdKyoten = string.Empty;
                CdBlock = "0";
            }

            TdUnitType = TdUnitType.TdCeiling;

            DtTekiyoKaishi = DateTime.Today;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdShain = string.Empty;
            NmShain = string.Empty;

            NuTdunitCnt = "1";
            NuThreshold = "1";
        }        

        // 参照日から情報取得
        private void SetReferenceInfo()
        {
            var tekiyoDate = ReferenceLog.GetStartDateInRange(ReferenceDate.ToString("yyyyMMdd"));
            var data = SeatThresholdLoader.GetFromKey(_seatThreshold.CdKyoten, _seatThreshold.CdBlock, tekiyoDate);

            if (data is not null)
            {
                TdUnitType = data.TdUnitType;
                NuTdunitCnt = data.NuTdunitCnt.ToString();
                NuThreshold= data.NuThreshold.ToString();

                DtTorokuNichiji = data.CreatedAt;
                DtKoshinNichiji = data.UpdatedAt;
                CdShain = _shainInfo.HenkoshaCode;
                NmShain = _shainInfo.HenkoshaName;
                DtTekiyoKaishi = DateTime.Parse(data.Tekiyokaishi.GetDate());
                DtTekiyoMuko = DateTime.Parse(data.TekiyoMuko.GetDate());
            }
            _isChange = false;
        }

        // 適用名称再取得
        private void ReloadTekiyoName()
        {
            TekiyoDate.ReferenceDate = ReferenceDate.ToString("yyyyMMdd");
            NmKyoten = NameLoader.GetKyoten(CdKyoten);
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
        private bool Regist()
        {
            try
            {
                if (!ValidateInput())
                {
                    return false;
                }

                var targetData = new ThresholdInfo
                {
                    CdKyoten = CdKyoten,
                    CdBlock = CdBlock.PadLeft(2, '0'),
                    TdUnitType = TdUnitType,
                    NuTdunitCnt = int.Parse(NuTdunitCnt),
                    NuThreshold = decimal.Parse(NuThreshold),

                    Tekiyokaishi = DtTekiyoKaishi.ToString("yyyyMMdd"),
                    TekiyoMuko = DtTekiyoMuko.ToString("yyyyMMdd"),
                };

                var existData = SeatThresholdLoader.GetFromKey(targetData.CdKyoten, targetData.CdBlock, targetData.Tekiyokaishi);
                var isExist = existData is not null;

                if (!ValidateSummaryDate(isExist))
                {
                    return false;
                }

                if (IsAdd)
                {
                    if (isExist)
                    {
                        MessageDialog.Show(_dialogService, $"同一組み合わせのデータが登録済みです\n拠点[{CdKyoten}],ブロック[{CdBlock}],適用開始日[{DtTekiyoKaishi.ToString("yyyyMMdd")}]\n", "入力エラー");
                        return false;
                    }

                    BlockEntityManager.Regist(targetData, _shainInfo);
                }
                else if (isExist)
                {
                    targetData.BlockId = existData!.BlockId;
                    BlockEntityManager.Update(targetData, _shainInfo);
                }
                else
                {
                    BlockEntityManager.Regist(targetData, _shainInfo);
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
            if(CdKyoten.IsNullOrEmpty() ||
                CdBlock.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点コード、ブロックを入力してください。", "入力エラー");
                return false;
            }

            if (NmKyoten.IsNullOrEmpty())
            {
                MessageDialog.Show(_dialogService, "拠点名称が取得出来ていません。", "入力エラー");
                return false;
            }

            if (!int.TryParse(NuTdunitCnt, out int tdUnitCnt))
            {
                MessageDialog.Show(_dialogService, "表示器数を入力してください。", "入力エラー");
                return false;
            }

            if (!decimal.TryParse(NuThreshold, out var threshold))
            {
                MessageDialog.Show(_dialogService, "しきい値を入力してください。", "入力エラー");
                return false;
            }

            if (threshold > 9999.9m)
            {
                MessageDialog.Show(_dialogService, "しきい値は9999.9以下で入力してください。", "入力エラー");
                return false;
            }

            return true;
        }

        // 適用期間チェック
        private bool ValidateSummaryDate(bool isUpdate)
        {
            try
            {
                ReferenceLog.LogInfos = LogLoader.Get(CdKyoten, CdBlock.PadLeft(2, '0')).ToList();
                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, isUpdate);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "入力エラー");
                return false;
            }
        }
    }
}
