using DistProg.Models;
using DistProg.Views;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using WindowLib.Utils;

namespace DistProg.ViewModels
{
    public class MainDistProgViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand Reload { get; }
        public DelegateCommand ShowDistUncompleted { get; }
        public DelegateCommand ShowDistCompleted { get; }
        public DelegateCommand Exit { get; }

        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _reloadTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Application.Current.Dispatcher);
        private string _dtDelivery = string.Empty;

        private DateTime _latestTime;
        public DateTime LatestTime
        {
            get => _latestTime;
            set => SetProperty(ref _latestTime, value);
        }

        private ObservableCollection<Models.DistProg> _distProgs = new ObservableCollection<Models.DistProg>();
        public ObservableCollection<Models.DistProg> DistProgs
        {
            get => _distProgs;
            set => SetProperty(ref _distProgs, value);
        }

        public MainDistProgViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:Reload");
                LoadDatas();
            });

            ShowDistUncompleted = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:ShowDistUncompleted");
                regionManager.RequestNavigate("ContentRegion", nameof(DistUncompleted), new NavigationParameters
                {
                    { "DtDelivery", _dtDelivery },
                });
            });

            ShowDistCompleted = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:ShowDistCompleted");
                regionManager.RequestNavigate("ContentRegion", nameof(DistCompleted), new NavigationParameters
                {
                    { "DtDelivery", _dtDelivery },
                });
            });

            Exit = new DelegateCommand(() =>
            {
                Syslog.Debug("MainDistProgViewModel:Exit");
                TimerStop();
                Application.Current.MainWindow.Close();
            });

            SelectDeliveryDate();
            LoadDatas();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistProgs, DistProgLoader.Get());
                LatestTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }

        private void SelectDeliveryDate()
        {
            _dialogService.ShowDialog(nameof(SelectDeliveryDateDlg),
                rc =>
                {
                    if (rc.Result != ButtonResult.OK)
                    {
                        Application.Current.MainWindow.Close();
                        return;
                    }

                    _dtDelivery = rc.Parameters.GetValue<DateTime>("Date").ToString("yyyyMMdd");
                });
        }

        private void TimerStart()
        {
            _reloadTimer.Tick += (s, e) => LoadDatas();
            _reloadTimer.Interval = new TimeSpan(0, 1, 0);
            _reloadTimer.Start();
        }

        private void TimerStop()
        {
            _reloadTimer.Stop();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            TimerStart();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            TimerStop();
        }
    }
}
