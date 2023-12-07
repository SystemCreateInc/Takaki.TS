using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SeatThreshold.Views;

namespace SeatThreshold
{
    internal class SeatThresholdModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainSeatThreshold));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainSeatThreshold>();

            // ダイアログ登録
            containerRegistry.RegisterForNavigation<InputSeatThresholdDlg>();
        }
    }
}
