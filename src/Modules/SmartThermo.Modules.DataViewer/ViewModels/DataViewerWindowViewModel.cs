using Prism.Regions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : RegionViewModelBase
    {
        public DataViewerWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications) 
            : base(regionManager, deviceConnector, notifications)
        {
            
        }
    }
}
