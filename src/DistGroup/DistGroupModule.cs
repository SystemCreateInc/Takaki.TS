using DistGroup.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DistGroup
{
    internal class DistGroupModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistGroup));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistGroup>();

            // ダイアログ登録
            containerRegistry.RegisterDialog<InputDistGroupDlg>();
        }
    }
}
