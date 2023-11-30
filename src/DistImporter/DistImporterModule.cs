﻿using DbLib.Defs;
using DbLib.Defs.DbLib.Defs;
using ImportLib.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreImporter
{
    public class DistImporterModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainImporter), new NavigationParameters
            {
                { "ImportType", ImportType.Dist }
            });
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
