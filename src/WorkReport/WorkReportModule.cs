using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using WorkReport.Views;

namespace WorkReport
{
    internal class WorkReportModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", nameof(MainWorkReport));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWorkReport>();
        }
    }
}
