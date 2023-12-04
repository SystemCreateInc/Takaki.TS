using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SeatMapping.Views;

namespace SeatMapping
{
    internal class SeatMappingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainSeatMapping));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainSeatMapping>();
        }
    }
}
