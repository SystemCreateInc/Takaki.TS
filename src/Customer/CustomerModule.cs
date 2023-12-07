using Customer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Customer
{
    internal class CustomerModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainCustomer));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainCustomer>();

            containerRegistry.RegisterDialog<InputCustomer>();
        }
    }
}
