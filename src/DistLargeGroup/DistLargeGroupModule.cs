using DistLargeGroup.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DistLargeGroup
{
    internal class DistLargeGroupModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistLargeGroup));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistLargeGroup>();

            // ダイアログ登録
            containerRegistry.RegisterDialog<InputDistLargeGroupDlg>();
        }
    }
}
