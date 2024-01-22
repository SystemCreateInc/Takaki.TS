using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using StowageListPrint.Models;
using WindowLib.Utils;

namespace StowageListPrint.ViewModels
{
    public class InputStowageDlgViewModel : BindableBase, IDialogAware
    {
        public DelegateCommand OK { get; }
        public DelegateCommand Cancel { get; }

        public string Title => "積付数変更画面";

        private readonly IDialogService _dialogService;
        public event Action<IDialogResult>? RequestClose;
        private Models.StowageListPrint _stowageListPrint = new Models.StowageListPrint();

        private string _cdCourse = string.Empty;
        public string CdCourse
        {
            get => _cdCourse;
            set => SetProperty(ref _cdCourse, value);
        }

        private int _cdRoute;
        public int CdRoute
        {
            get => _cdRoute;
            set => SetProperty(ref _cdRoute, value);
        }

        private string _cdTokuisaki = string.Empty;
        public string CdTokuisaki
        {
            get => _cdTokuisaki;
            set => SetProperty(ref _cdTokuisaki, value);
        }

        private string _nmTokuisaki = string.Empty;
        public string NmTokuisaki
        {
            get => _nmTokuisaki;
            set => SetProperty(ref _nmTokuisaki, value);
        }

        private int _largeBox;
        public int LargeBox
        {
            get => _largeBox;
            set => SetProperty(ref _largeBox, value);
        }

        private int? _newLargeBoxPs;
        public int? NewLargeBoxPs
        {
            get => _newLargeBoxPs;
            set => SetProperty(ref _newLargeBoxPs, value);
        }

        private int _smallBox;
        public int SmallBox
        {
            get => _smallBox;
            set => SetProperty(ref _smallBox, value);
        }

        private int? _newSmallBoxPs;
        public int? NewSmallBoxPs
        {
            get => _newSmallBoxPs;
            set => SetProperty(ref _newSmallBoxPs, value);
        }

        private int _blueBox;
        public int BlueBox
        {
            get => _blueBox;
            set => SetProperty(ref _blueBox, value);
        }

        private int? _newBlueBoxPs;
        public int? NewBlueBoxPs
        {
            get => _newBlueBoxPs;
            set => SetProperty(ref _newBlueBoxPs, value);
        }

        private int _etcBox;
        public int EtcBox
        {
            get => _etcBox;
            set => SetProperty(ref _etcBox, value);
        }

        private int? _newEtcBoxPs;
        public int? NewEtcBoxPs
        {
            get => _newEtcBoxPs;
            set => SetProperty(ref _newEtcBoxPs, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private int _selectShainIndex = -1;
        public int SelectShainIndex
        {
            get => _selectShainIndex;
            set => SetProperty(ref _selectShainIndex, value);
        }

        private List<ShainCombo> _shainList = new List<ShainCombo>();
        public List<ShainCombo> ShainList
        {
            get => _shainList;
            set => SetProperty(ref _shainList, value);
        }

        public InputStowageDlgViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            OK = new DelegateCommand(() =>
            {
                Syslog.Debug("InputStowageDlgViewModel:OK");
                ErrorMessage = string.Empty;

                if (Register())
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });

            Cancel = new DelegateCommand(() =>
            {
                Syslog.Debug("InputStowageDlgViewModel:Cancel");
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }


        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _stowageListPrint = parameters.GetValue<Models.StowageListPrint>("CurrentStowageListPrint");
            SetShainList(parameters.GetValue<string>("DtDelivery"));
            InitDialog();
        }

        private void InitDialog()
        {
            ErrorMessage = string.Empty;
            CdCourse = _stowageListPrint.CdCourse;
            CdRoute = _stowageListPrint.CdRoute;
            CdTokuisaki = _stowageListPrint.CdTokuisaki;
            NmTokuisaki = _stowageListPrint.NmTokuisaki;

            // 左側は予定数を表示
            LargeBox = _stowageListPrint.LargeBoxOps;
            SmallBox = _stowageListPrint.SmallBoxOps;
            BlueBox = _stowageListPrint.BlueBoxOps;
            EtcBox = _stowageListPrint.EtcBoxOps;

            // 右側は実績数を表示
            NewLargeBoxPs = _stowageListPrint.LargeBoxRps;
            NewSmallBoxPs = _stowageListPrint.SmallBoxRps;
            NewBlueBoxPs = _stowageListPrint.BlueBoxRps;
            NewEtcBoxPs = _stowageListPrint.EtcBoxRps;
        }

        private bool Register()
        {
            try
            {
                if (NewLargeBoxPs == null)
                {
                    MessageDialog.Show(_dialogService, "厚箱を入力して下さい。", "入力エラー");
                    return false;
                }

                if (NewSmallBoxPs == null)
                {
                    MessageDialog.Show(_dialogService, "薄箱を入力して下さい。", "入力エラー");
                    return false;
                }

                if (NewBlueBoxPs == null)
                {
                    MessageDialog.Show(_dialogService, "青箱を入力して下さい。", "入力エラー");
                    return false;
                }

                if (NewEtcBoxPs == null)
                {
                    MessageDialog.Show(_dialogService, "その他を入力して下さい。", "入力エラー");
                    return false;
                }

                if (SelectShainIndex == -1)
                {
                    MessageDialog.Show(_dialogService, "作業社員コードを選択して下さい。", "入力エラー");
                    return false;
                }

                StowageManager.Update(_stowageListPrint.IdStowages, (int)NewLargeBoxPs, (int)NewSmallBoxPs, (int)NewBlueBoxPs, (int)NewEtcBoxPs,
                    ShainList[SelectShainIndex].Id, ShainList[SelectShainIndex].Name);
                return true;
            }
            catch (Exception ex)
            {
                MessageDialog.Show(_dialogService, ex.Message, "エラー");
                return false;
            }
        }

        private void SetShainList(string deliveryDate)
        {
            ShainList = ComboCreator.GetShain(deliveryDate);

            var henkoShain = ShainList.FirstOrDefault(x => x.Id == _stowageListPrint.HenkoshaCode.Trim());
            if (henkoShain is not null)
            {
                SelectShainIndex = ShainList.IndexOf(henkoShain);
            }
        }
    }
}
