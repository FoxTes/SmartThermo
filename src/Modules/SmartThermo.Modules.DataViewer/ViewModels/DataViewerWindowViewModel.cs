using System.Windows;
using Prism.Regions;
using SmartThermo.Core;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : ViewModelBase
    {
        private readonly IDeviceConnector _deviceConnector;
        private readonly IRegionManager _regionManager;
        
        public DataViewerWindowViewModel(IDeviceConnector deviceConnector, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _deviceConnector = deviceConnector;
            _deviceConnector.StatusConnectChanged += OnStatusConnectChanged;
        }

        private void OnStatusConnectChanged(object stage, StatusConnect args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _regionManager.RequestNavigate(RegionNames.DataViewerContent,
                    _deviceConnector.StatusConnect == StatusConnect.Connected
                        ? "LoadDataViewerWindow"
                        : "NoLoadDataViewerWindow");
            });
        }
    }
}
