using DistListPrint.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchBoxLib.Views;
using SelDistGroupLib.Views;

namespace DistListPrint
{
    internal class DistListPrintModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistListPrint));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistListPrint>();
            containerRegistry.RegisterDialog<SelDistGroupDlg>();
            containerRegistry.RegisterDialog<SearchBoxDlg>();
        }
    }
}
