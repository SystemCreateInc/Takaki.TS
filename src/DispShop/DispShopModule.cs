using DispShop.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SelDistGroupLib.Views;
using TdDpsLib.Models;
using WindowLib;

namespace DispShop
{
    internal class DispShopModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainDispShop));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainDispShop>();
            containerRegistry.RegisterDialog<SelDistGroupDlg>();

            // シングルトン登録
            containerRegistry.RegisterSingleton<TdDpsManager>();
        }
    }
}
