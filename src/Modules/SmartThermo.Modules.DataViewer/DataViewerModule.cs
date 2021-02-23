using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SmartThermo.Core;
using SmartThermo.Modules.DataViewer.Views;

namespace SmartThermo.Modules.DataViewer
{
    public class DataViewerModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public DataViewerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MainContent, "DataViewerWindow");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DataViewerWindow>();
        }
    }
}