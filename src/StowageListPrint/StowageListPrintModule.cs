using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using StowageListPrint.Views;

namespace StowageListPrint
{
    internal class StowageListPrintModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainStowageListPrint));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainStowageListPrint>();

            // ダイアログ登録
            //containerRegistry.RegisterDialog<InputStowageDlg>();
        }
    }
}
