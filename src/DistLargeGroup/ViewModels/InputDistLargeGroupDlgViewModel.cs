using DistLargeGroup.Infranstructures;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using ReferenceLogLib;
using TakakiLib.Models;
using WindowLib.Utils;

namespace DistLargeGroup.ViewModels
{
    public class InputDistLargeGroupDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand ClearCommand { get; }
        public DelegateCommand RegistCommand { get; }
        public DelegateCommand BackCommand { get; }
        public DelegateCommand ReferCommand { get; }
        public DelegateCommand ReleaseCommand { get; }

        public string Title => "大仕分グループ情報入力";

        public event Action<IDialogResult>? RequestClose;
        private Models.DistLargeGroup? _distLargeGroup;
        private IDialogService _dialogService;
        private bool _isModified;
        private ShainInfo? _shain;

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
                UpdateKyotenName();
                _isModified = true;
            }
        }

        // 拠点名称
        private string _nmKyoten = string.Empty;
        public string NmKyoten
        {
            get => _nmKyoten;
            set
            {
                SetProperty(ref _nmKyoten, value);
                _isModified = true;
            }
        }

        // 大仕分グループ
        private string _cdLargeGroup = string.Empty;
        public string CdLargeGroup
        {
            get => _cdLargeGroup;
            set
            {
                SetProperty(ref _cdLargeGroup, value);
                _isModified = true;
            }
        }

        // 大仕分グループ名称
        private string _cdLargeGroupName = string.Empty;
        public string CdLargeGroupName
        {
            get => _cdLargeGroupName;
            set
            {
                SetProperty(ref _cdLargeGroupName, value);
                _isModified = true;
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
                _isModified = true;
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
                _isModified = true;
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
        private string _cdHenkosha = string.Empty;
        public string CdHenkosha
        {
            get => _cdHenkosha;
            set => SetProperty(ref _cdHenkosha, value);
        }

        // 更新者名称
        private string _nmHenkosha = string.Empty;
        public string NmHenkosha
        {
            get => _nmHenkosha;
            set => SetProperty(ref _nmHenkosha, value);
        }

        private bool _isDateRelease;
        public bool IsDateRelease
        {
            get => _isDateRelease;
            set => SetProperty(ref _isDateRelease, value);
        }


        // 履歴表示リスト
        private ReferenceLog _referenceLog = new ReferenceLog();
        public ReferenceLog ReferenceLog
        {
            get => _referenceLog;
            set => SetProperty(ref _referenceLog, value);
        }

        public DateTime? _lastReferenceDate;

        private bool _isAdd;
        public bool IsAdd
        {
            get => _isAdd;
            set => SetProperty(ref _isAdd, value);
        }

        private bool _isEdit = false;
        public bool IsEdit
        {
            get => _isEdit;
            set => SetProperty(ref _isEdit, value);
        }

        public InputDistLargeGroupDlgViewModel(IDialogService dialogService)
        {
            ClearCommand = new DelegateCommand(Clear);
            RegistCommand = new DelegateCommand(Regist);
            BackCommand = new DelegateCommand(Back);
            ReferCommand = new DelegateCommand(() => Refer(false));
            ReleaseCommand = new DelegateCommand(Release);
            _dialogService = dialogService;
        }

        private void Release()
        {
            IsDateRelease = true;
        }

        private void Refer(bool isInit)
        {

            try
            {
                var log = ReferenceLog.GetLogByDateAndSelect(ReferenceDate);
                if (log is null)
                {
                    Clear();

                    if (!isInit)
                    {
                        MessageDialog.Show(_dialogService, "参照する履歴はありません", "該当適用期間無し");
                        if (_lastReferenceDate is not null)
                        {
                            ReferenceDate = _lastReferenceDate.GetValueOrDefault();
                            Refer(true);
                        }
                    }
                        
                    return;
                }

                _lastReferenceDate = ReferenceDate;

                var largeDistGroup = LargeGroupRepository.FindById(log.Id);
                if (largeDistGroup is null)
                {
                    Clear();
                    return;
                }

                DtTekiyoKaishi = largeDistGroup.DtTekiyoKaishi;
                CdLargeGroupName = largeDistGroup.CdLargeGroupName;
                DtTekiyoMuko = largeDistGroup.DtTekiyoMuko;
                DtTorokuNichiji = largeDistGroup.CreatedAt;
                DtKoshinNichiji = largeDistGroup.UpdatedAt;
                CdHenkosha = largeDistGroup.CdHenkosha;
                NmHenkosha = largeDistGroup.NmHenkosha;
                _isModified = false;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private void Back()
        {
            RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
        }

        private void Regist()
        {
            try
            {
                // idがnullの時は追加
                long? id = null;
                var log = ReferenceLog.GetLogByDateAndSelect(DtTekiyoKaishi);
                if (log is not null)
                {
                    id = LargeGroupRepository.FindById(log.Id)?.IdLargeGroup;
                }

                if (!ValidateInput())
                {
                    return;
                }

                ReferenceLog.ValidateSummaryDate(DtTekiyoKaishi, DtTekiyoMuko, log is not null);

                if (IsAdd && LargeGroupRepository.IsExist(CdLargeGroup))
                {
                    MessageDialog.Show(_dialogService, "同じ大仕分グループがすでに登録されているため登録できません", "入力エラー");
                    return;
                }

                var largeGroup = new DistLargeGroup.Models.DistLargeGroup
                {
                    IdLargeGroup = id ?? 0,
                    CdKyoten = CdKyoten,
                    CdLargeGroup = CdLargeGroup,
                    CdLargeGroupName = CdLargeGroupName ?? "",
                    DtTekiyoKaishi = DtTekiyoKaishi,
                    DtTekiyoMuko = DtTekiyoMuko,
                    CdHenkosha = _shain?.HenkoshaCode ?? "",
                    NmHenkosha= _shain?.HenkoshaName ?? "",
                };

                LargeGroupRepository.Save(largeGroup);
                _isModified = false;
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(CdKyoten.Trim()))
            {
                MessageDialog.Show(_dialogService, "拠点コードを入力してください", "入力エラー");
                return false;
            }

            if (string.IsNullOrEmpty(CdLargeGroup.Trim())
                || string.IsNullOrEmpty(CdLargeGroupName.Trim()))
            {
                MessageDialog.Show(_dialogService, "大仕分グループコード、名称を入力してください", "入力エラー");
                return false;
            }

            if (string.IsNullOrEmpty(NmKyoten))
            {
                MessageDialog.Show(_dialogService, "拠点名称が取得出来ていません", "入力エラー");
                return false;
            }

            return true;
        }

        private void Clear()
        {
            SetupForAdd();
        }

        public bool CanCloseDialog()
        {
            if (_isModified
                && MessageDialog.Show(_dialogService, "変更された情報が登録されていません。\n一覧画面に戻りますか？", "変更確認", ButtonMask.Yes | ButtonMask.No) != ButtonResult.Yes)
            {
                return false;
            }

            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _distLargeGroup = parameters.GetValue<Models.DistLargeGroup>("DistLargeGroup");
            _shain = parameters.GetValue<ShainInfo>("Shain");
            IsAdd = _distLargeGroup == null;
            IsEdit = !IsAdd;
            InitDialog();
            _isModified = false;
        }

        private void InitDialog()
        {
            ReferenceDate = DateTime.Today;
            ReferenceLog.LogInfos.Clear();

            SetupForAdd();
            if (!_isAdd && _distLargeGroup is not null)
            {
                ReferenceLog.LogInfos = LargeGroupQueryService.GetLog(_distLargeGroup.CdLargeGroup).ToList();
                // 当日で参照
                Refer(true);
            }
        }

        private void SetupForAdd()
        {
            if (!IsAdd)
            {
                CdKyoten = _distLargeGroup!.CdKyoten;
                CdLargeGroup = _distLargeGroup.CdLargeGroup;
                CdLargeGroupName = _distLargeGroup.CdLargeGroupName;
            }
            else
            {
                CdKyoten = string.Empty;
                CdLargeGroup = string.Empty;
                CdLargeGroupName = string.Empty;
            }

            CdLargeGroupName = string.Empty;

            DtTekiyoKaishi = DateTime.Now;
            DtTekiyoMuko = new DateTime(2999, 12, 31);
            DtTorokuNichiji = null;
            DtKoshinNichiji = null;
            CdHenkosha = string.Empty;
            NmHenkosha = string.Empty;
            _isModified = false;
        }

        private void UpdateKyotenName()
        {
            try
            {
                NameLoader.selectDate = ReferenceDate.ToString("yyyyMMdd");
                NmKyoten = NameLoader.GetKyoten(CdKyoten);
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
            }
        }
    }
}
