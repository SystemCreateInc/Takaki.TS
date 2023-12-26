using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using EnvironSetting.Views;

namespace EnvironSetting
{
    internal class EnvironSettingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainEnvironSetting));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainEnvironSetting>();
        }
    }
}
