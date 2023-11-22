using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SetHenkosha.Views;

namespace SetHenkosha
{
    internal class SetHenkoshaModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainSetHenkosha));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainSetHenkosha>();
        }
    }
}
