using LargeDist.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LargeDist
{
    public class LargeDistModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainLargeDist));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainLargeDist>();
            containerRegistry.RegisterForNavigation<ItemScan>();
            containerRegistry.RegisterForNavigation<ItemList>();
            containerRegistry.RegisterForNavigation<BlockLargeDist>();
            containerRegistry.RegisterForNavigation<ItemLargeDist>();
            containerRegistry.RegisterForNavigation<LargeDistItemList>();
            containerRegistry.RegisterForNavigation<LargeDistCustomerList>();
            containerRegistry.RegisterForNavigation<ModifyQtyDialog>();
            containerRegistry.RegisterForNavigation<ModifyBoxUnitDialog>();
            containerRegistry.RegisterForNavigation<CompletedDialog>();
            containerRegistry.RegisterForNavigation<CancelDistDialog>();
            containerRegistry.RegisterForNavigation<SelectItemDialog>();
            containerRegistry.RegisterForNavigation<ScanOrderConfig>();
        }
    }
}
