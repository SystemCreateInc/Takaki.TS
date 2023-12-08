using Picking.Models;
using Picking.ViewModels;
using Picking.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SelDistGroupLib.Models;
using SelDistGroupLib.Views;
using TdDpsLib.Models;
using WindowLib.Views;

namespace Picking
{
    public class PickingModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainPicking));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainPicking>();
            containerRegistry.RegisterForNavigation<DistInfoWindow>();
            containerRegistry.RegisterForNavigation<DistDetailWindow>();
            containerRegistry.RegisterForNavigation<DistItemScanWindow>();

            // 仕分けグループ選択
            containerRegistry.RegisterDialog<SelDistGroupDlg>();

            // 進捗登録
            containerRegistry.RegisterDialog<ProgressDialog>();

            // 社員
            containerRegistry.RegisterDialog<ShainDlg>();
            // 数量変更
            containerRegistry.RegisterDialog<ChangeQtyDlg>();
            // 商品選択
            containerRegistry.RegisterDialog<SelectItemDlg>();


            // シングルトン登録
            containerRegistry.RegisterSingleton<TdDpsManager>();
            containerRegistry.RegisterSingleton<DistColorInfo>();
            containerRegistry.RegisterSingleton<DistGroupEx>();
        }
    }
}
