using ExportBase;
using DistExpoter.Views;
using LogLib;
using Prism.Ioc;
using Prism.Modularity;
using StoreExporter;
using System.Windows;
using WindowLib;

namespace DistExpoter
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
            moduleCatalog.AddModule<ExportBaseModule>();
            moduleCatalog.AddModule<DistExporterModule>();
        }
    }
}
