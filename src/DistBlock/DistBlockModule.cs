using DistBlock.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DistBlock
{
    internal class DistBlockModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistBlock));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistBlock>();
            containerRegistry.RegisterForNavigation<InputDistBlock>();
        }
    }
}
