using DbLib.Extensions;
using DistProg.Models;
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
    public class DistUncompletedViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand Reload { get; }
        public DelegateCommand Back { get; }

        private readonly IDialogService _dialogService;
        private readonly DispatcherTimer _reloadTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Application.Current.Dispatcher);

        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set
            {
                SetProperty(ref _dtDelivery, value);
                DispDtDelivery = DtDelivery.GetDate();
            }
        }

        private string _dispDtDelivery = string.Empty;
        public string DispDtDelivery
        {
            get => _dispDtDelivery;
            set => SetProperty(ref _dispDtDelivery, value);
        }

        private DateTime _latestTime;
        public DateTime LatestTime
        {
            get => _latestTime;
            set => SetProperty(ref _latestTime, value);
        }

        private ObservableCollection<Models.DistProg> _distUncompleteds = new ObservableCollection<Models.DistProg>();
        public ObservableCollection<Models.DistProg> DistUncompleteds
        {
            get => _distUncompleteds;
            set => SetProperty(ref _distUncompleteds, value);
        }

        public DistUncompletedViewModel(IDialogService dialogService, IRegionManager regionManager)
        {
            _dialogService = dialogService;

            Reload = new DelegateCommand(() =>
            {
                Syslog.Debug("DistUncompletedViewModel:Reload");
                LoadDatas();
            });

            Back = new DelegateCommand(() =>
            {
                Syslog.Debug("DistUncompletedViewModel:Back");
                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _reloadTimer.Stop();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            DtDelivery = navigationContext.Parameters.GetValue<string>("DtDelivery");
            LoadDatas();
            _reloadTimer.Tick += (s, e) => LoadDatas();
            _reloadTimer.Interval = new TimeSpan(0, 1, 0);
            _reloadTimer.Start();
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistUncompleteds, DistProgLoader.GetUncompleteds(DtDelivery));
                LatestTime = DateTime.Now;
            }
            catch (Exception e)
            {
                Syslog.Error($"LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }
        }
    }
}
