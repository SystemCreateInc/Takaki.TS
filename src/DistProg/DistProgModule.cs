using DistProg.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DistProg
{
    internal class DistProgModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDistProg));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDistProg>();
            containerRegistry.RegisterForNavigation<DistUncompleted>();
            containerRegistry.RegisterForNavigation<DistCompleted>();

            containerRegistry.RegisterDialog<SelectDeliveryDateDlg>();
        }
    }
}
