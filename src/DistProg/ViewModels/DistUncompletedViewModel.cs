using DbLib.Extensions;
using DistProg.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using WindowLib.Utils;

namespace DistProg.ViewModels
{
    public class DistUncompletedViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand Reload { get; }
        public DelegateCommand Back { get; }

        private readonly IDialogService _dialogService;

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
                // fixme:更新ボタン
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
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            LoadDatas();

            // fixme:納品日
            DtDelivery = DateTime.Today.ToString("yyyyMMdd");
        }

        private void LoadDatas()
        {
            try
            {
                CollectionViewHelper.SetCollection(DistUncompleteds, DistProgLoader.GetUncompleteds());
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
