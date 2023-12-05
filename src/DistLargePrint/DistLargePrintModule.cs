using DistLargePrint.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DistLargePrint
{
    internal class DistLargePrintModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistLargePrint));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistLargePrint>();
        }
    }
}
