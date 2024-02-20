using Mapping.ViewModels;
using Mapping.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Mapping
{
    internal class MappingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainMapping));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainMapping>();
            containerRegistry.RegisterForNavigation<OverTokuisaki>();
            containerRegistry.RegisterForNavigation<LocInfo>();
            containerRegistry.RegisterForNavigation<DpsOtherInfo>();

            containerRegistry.RegisterDialog<DeliveryDateDialog>();
            containerRegistry.RegisterDialog<RackAllocMaxDialog>();
        }
    }
}
