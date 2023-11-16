using Prism.Ioc;
using Prism.Modularity;
using SearchBoxLib.Views;

namespace SearchBoxLib
{
    public class SearchBoxLibModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SearchBoxDlg>();
        }
    }
}
