using DbLib.Extensions;
using Mapping.Models;
using LogLib;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System.Collections.ObjectModel;
using System.Windows;
using WindowLib.Utils;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using Mapping.Views;
using ControlzEx.Standard;
using Prism.Events;
using Prism.Regions;

namespace Mapping.ViewModels
{
    public class OverTokuisakiViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnPrint { get; }
        public DelegateCommand OnBack { get; }

        private int _tokuisakiCnt = 0;
        public int TokuisakiCnt
        {
            get => _tokuisakiCnt;
            set => SetProperty(ref _tokuisakiCnt, value);
        }
        private ObservableCollection<OverInfo> _overinfos = new ObservableCollection<OverInfo>();
        public ObservableCollection<OverInfo> OverInfos
        {
            get => _overinfos;
            set => SetProperty(ref _overinfos, value);
        }

        private DistGroupInfo? _distgroupinfo = null;
        private MappingManager? _mapping = null;

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;

        public OverTokuisakiViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            OnPrint = new DelegateCommand(() =>
            {
                Syslog.Debug("OverTokuisakiViewModel:OverPrint");
            });


            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】OverTokuisakiViewModel:OnBack");

                regionManager.Regions["ContentRegion"].NavigationService.Journal.GoBack();
            });

        }
        private void Closing(object? sender, CancelEventArgs e)
        {
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Syslog.Info($"OverTokuisakiViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _mapping = navigationContext.Parameters.GetValue<MappingManager>("Mapping");
            _distgroupinfo = navigationContext.Parameters.GetValue<DistGroupInfo>("currentdistinfo");

            LoadDatas();

            Syslog.Info($"OverTokuisakiViewModel:OnNavigatedTo");
        }

        public void LoadDatas()
        {

        }
    }
}
