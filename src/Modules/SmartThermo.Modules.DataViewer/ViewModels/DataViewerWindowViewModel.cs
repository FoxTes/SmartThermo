using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.Notifications;

namespace SmartThermo.Modules.DataViewer.ViewModels
{
    public class DataViewerWindowViewModel : RegionViewModelBase
    {
        public DataViewerWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            DeviceConnector.StatusConnectChanged += (_, connect) =>
            {
                regionManager.RequestNavigate(RegionNames.DataViewerContent,
                    DeviceConnector.StatusConnect == StatusConnect.Connected
                        ? "LoadDataViewerWindow"
                        : "NoLoadDataViewerWindow");
            };
        }
    }
}
