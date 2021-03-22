using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SmartThermo.Core;
using SmartThermo.Modules.DataViewer.ViewModels;
using SmartThermo.Modules.DataViewer.ViewModels.Represent;
using SmartThermo.Modules.DataViewer.Views;
using SmartThermo.Modules.DataViewer.Views.Represent;

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
            _regionManager.RequestNavigate(RegionNames.DataViewerContent, "NoLoadDataViewerWindow");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<DataViewerWindow, DataViewerWindowViewModel>();      
            containerRegistry.RegisterForNavigation<LoadDataViewerWindow, LoadDataViewerWindowViewModel>();
            containerRegistry.RegisterForNavigation<NoLoadDataViewerWindow>();
        }
    }
}