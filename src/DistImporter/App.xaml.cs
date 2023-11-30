using DistImporter.Views;
using ImportLib;
using LogLib;
using Prism.Ioc;
using Prism.Modularity;
using StoreImporter;
using System.Windows;
using WindowLib;

namespace DistImporter
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
            moduleCatalog.AddModule<DistImporterModule>();
        }
    }
}
