﻿using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SearchBoxLib.Views;
using TakahataDistListPrint.Views;

namespace TakahataDistListPrint
{
    internal class TakahataDistListPrintModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainTakahataDistListPrint));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainTakahataDistListPrint>();
            containerRegistry.RegisterDialog<SelectDeliveryDateDlg>();
            containerRegistry.RegisterDialog<SearchBoxDlg>();
        }
    }
}
