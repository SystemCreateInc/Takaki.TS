using ImportLib;
using LogLib;
using MasterImporter.Views;
using Prism.Ioc;
using Prism.Modularity;
using StoreImporter;
using System.Windows;
using WindowLib;

namespace MasterImporter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            Syslog.Init();
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<WindowLibModule>();
            moduleCatalog.AddModule<ImportLibModule>();
            moduleCatalog.AddModule<MasterImporterModule>();
        }
    }
}
