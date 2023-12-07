using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SeatThreshold.Models;
using SeatThreshold.Views;
using System.Collections.ObjectModel;
using System.Windows;
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

        private ObservableCollection<Models.SeatThreshold> _seatThresholds = new ObservableCollection<Models.SeatThreshold>();
        public ObservableCollection<Models.SeatThreshold> SeatThresholds
        {
            get => _seatThresholds;
            set => SetProperty(ref _seatThresholds, value);
        }

        private Models.SeatThreshold? _currentSeatThreshold;
        public Models.SeatThreshold? CurrentSeatThreshold
        {
            get => _currentSeatThreshold;
            set
            {
                SetProperty(ref _currentSeatThreshold, value);
                CanEdit = CurrentSeatThreshold != null;
            }
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }


        public MainSeatThresholdViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;

            Add = new DelegateCommand(() =>
            {
                Syslog.Debug("MainSeatThresholdViewModel:Add");
                if (ShowInputDialog(new Models.SeatThreshold()))
                {
                    LoadDatas();
                }
            });

            Edit = new DelegateCommand(() =>
            {
                if (CurrentSeatThreshold == null)
                {
                    return;
                }

                Syslog.Debug("MainSeatThresholdViewModel:Edit");
                if (ShowInputDialog(CurrentSeatThreshold))
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

            // fixme:社員コード + 社員名称
            Shain = "0000033550" + "　" + "小田賢行";

            LoadDatas();
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

        private bool ShowInputDialog(Models.SeatThreshold seatThreshold)
        {
            IDialogResult? result = null;

            _dialogService.ShowDialog(
                nameof(InputSeatThresholdDlg),
                new DialogParameters
                {
                    { "SeatThreshold", seatThreshold },
                },
                r => result = r);

            return result?.Result == ButtonResult.OK;
        }
    }
}
