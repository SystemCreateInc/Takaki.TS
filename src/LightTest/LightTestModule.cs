using LightTest.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WindowLib.Views;

namespace LightTest
{
    public class LightTestModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainLightTest));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainLightTest>();

            // 進捗登録
            containerRegistry.RegisterDialog<ProgressDialog>();
            // ダイアログ登録
            containerRegistry.RegisterDialog<SettingsDlg>();
        }
    }
}
