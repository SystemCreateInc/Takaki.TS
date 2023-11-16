using ColumnVisibilityLib.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace ColumnVisibilityLib
{
    public class ColumnVisibilityLibModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SelectColumnVisibilityDlg>();
        }
    }
}
