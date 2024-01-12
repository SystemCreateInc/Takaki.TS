using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SeatThreshold.Loader;
using SeatThreshold.Models;
using SeatThreshold.Views;
using System.Collections.ObjectModel;
using System.Windows;
using TakakiLib.Models;
using WindowLib.Utils;

namespace SeatThreshold.ViewModels
{
    public class MainSeatThresholdViewModel : BindableBase
    {
        public DelegateCommand Add { get; }
        public DelegateCommand Edit { get; }
        public DelegateCommand Exit { get; }
        public DelegateCommand LeftDoubleClick { get; }

        private readonly IDialogService _dialogService;

        private string _shain = string.Empty;
        public string Shain
        {
            get => _shain;
            set => SetProperty(ref _shain, value);
        }

        private ObservableCollection<ThresholdInfo> _seatThresholds = new ObservableCollection<ThresholdInfo>();
        public ObservableCollection<ThresholdInfo> SeatThresholds
        {
            get => _seatThresholds;
            set => SetProperty(ref _seatThresholds, value);
        }

        private ThresholdInfo? _currentSeatThreshold;
        public ThresholdInfo? CurrentSeatThreshold
        {
            get => _currentSeatThreshold;
            set
            {
                SetProperty(ref _currentSeatThreshold, value);
                CanEdit = CurrentSeatThreshold != null && _shainInfo is not null;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }

        private bool _isSelectedShain = false;
        public bool IsSelectedShain
        {
            get => _isSelectedShain;
            set => SetProperty(ref _isSelectedShain, value);
        }

        private ShainInfo? _shainInfo = new ShainInfo();

        public MainSeatThresholdViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            SetShain();
            LoadDatas();

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatThresholdViewModel:Add");
                if (ShowInputDialog(new ThresholdInfo()))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => IsSelectedShain);

            Edit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatThresholdViewModel:Edit");
                if (ShowInputDialog(CurrentSeatThreshold!))
                {
                    LoadDatas();
                }
            }).ObservesCanExecute(() => CanEdit);

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatThresholdViewModel:Exit");
                Application.Current.MainWindow.Close();
            });

            LeftDoubleClick = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatThresholdViewModel:LeftDoubleClick");
                Edit.Execute();
            }).ObservesCanExecute(() => CanEdit);
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(SeatThresholds, SeatThresholdLoader.Get());
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private bool ShowInputDialog(ThresholdInfo seatThreshold)
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputSeatThresholdDlg),
                new DialogParameters
                {
                    { "SeatThreshold", seatThreshold },
                    { "ShainInfo", _shainInfo },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }

        private void SetShain()
        {
            _shainInfo = ShainLoader.Get();

            IsSelectedShain = _shainInfo is not null;
            // 社員情報取得失敗で終了
            if (!IsSelectedShain)
            {
                MessageDialog.Show(_dialogService, $"変更社員情報が選択されていません", "エラー");
                Application.Current.MainWindow.Close();
                return;
            }
            Shain = $"{_shainInfo?.HenkoshaCode}  {_shainInfo?.HenkoshaName}";
        }
    }
}
