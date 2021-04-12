using Prism.Regions;
using SmartThermo.Core;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : ViewModelBase
    {
        public DataViewerWindowViewModel(IDeviceConnector deviceConnector, IRegionManager regionManager)
        {
            deviceConnector.StatusConnectChanged += (_, connect) =>
            {
                regionManager.RequestNavigate(RegionNames.DataViewerContent,
                    deviceConnector.StatusConnect == StatusConnect.Connected
                        ? "LoadDataViewerWindow"
                        : "NoLoadDataViewerWindow");
            };
        }
    }
}
