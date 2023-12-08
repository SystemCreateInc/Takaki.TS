using ImportLib.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowLib;

namespace ImportLib
{
    public class ImportLibModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<WindowLibModule>();
            containerRegistry.RegisterForNavigation<MainImporter>();
            containerRegistry.RegisterDialog<ImportDlg>();
        }
    }
}
