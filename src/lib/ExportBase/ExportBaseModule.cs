using ExportBase.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ExportBase
{
    public class ExportBaseModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainExporter>();
            containerRegistry.RegisterDialog<ExportDlg>();
            containerRegistry.RegisterDialog<ExportSettingsDlg>();
        }
    }
}
