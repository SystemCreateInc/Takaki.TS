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
using System.Printing;
using Mapping.Reports;
using PrintPreviewLib;
using PrintLib;
using DbLib.Defs;
using ReferenceLogLib.Models;

namespace Mapping.ViewModels
{
    public class DpsOtherInfoViewModel : BindableBase, INavigationAware
    {
        public DelegateCommand OnBack { get; }

        private string _dtDelivery = string.Empty;
        public string DtDelivery
        {
            get => _dtDelivery;
            set => SetProperty(ref _dtDelivery, value);
        }
        private string _dispdtDelivery = string.Empty;
        public string DispDtDelivery
        {
            get => _dispdtDelivery;
            set => SetProperty(ref _dispdtDelivery, value);
        }

        private ObservableCollection<Models.DpsOtherInfo> _dpsotherinfos = new ObservableCollection<Models.DpsOtherInfo>();
        public ObservableCollection<Models.DpsOtherInfo> DpsOtherInfos
        {
            get => _dpsotherinfos;
            set => SetProperty(ref _dpsotherinfos, value);
        }

        private readonly IRegionManager _regionManager;
        private readonly IDialogService _dialogService;

        public DpsOtherInfoViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            _regionManager = regionManager;
            _dialogService = dialogService;

            OnBack = new DelegateCommand(() =>
            {
                Syslog.Info("【戻る】DpsOtherInfoViewModel:OnBack");

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
            Syslog.Info($"DpsOtherInfoViewModel:OnNavigatedFrom");
            return;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            DtDelivery = navigationContext.Parameters.GetValue<string>("DtDelivery");
            DispDtDelivery = navigationContext.Parameters.GetValue<string>("DispDtDelivery");

            try
            {
                // 画面のソート順をデフォルトへ戻すため一旦クリア
                DpsOtherInfos = new ObservableCollection<Models.DpsOtherInfo>();
                CollectionViewHelper.SetCollection(DpsOtherInfos, MappingLoader.GetDpsOther(DtDelivery));
            }
            catch (Exception e)
            {
                Syslog.Error($"LocInfoViewModel:LoadDatas:{e.Message}");
                MessageDialog.Show(_dialogService, e.Message, "エラー");
            }

            Syslog.Info($"LocInfoViewModel:OnNavigatedTo");
        }
    }
}
