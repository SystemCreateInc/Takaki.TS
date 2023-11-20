using SelDistGroupLib.Views;
using Prism.Ioc;
using Prism.Modularity;

namespace SelDistGroupLib
{
    public class SelDistGroupLibModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SelDistGroupDlg>();
        }
    }
}
