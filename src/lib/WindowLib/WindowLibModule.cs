using Prism.Ioc;
using Prism.Modularity;
using WindowLib.Utils;

namespace WindowLib
{
    public class WindowLibModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // 外部スタイル読み込み
            StyleLoader.Initialize();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<Views.MessageDialog>();
            containerRegistry.RegisterDialog<Views.TabletMessageDialog>();
            containerRegistry.RegisterDialog<Views.ProgressRing>();
            containerRegistry.RegisterDialog<Views.ProgressDialog>();
        }
    }
}
    