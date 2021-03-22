using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;

namespace SmartThermo.Modules.Analytics.ViewModels
{
    public class AnalyticsWindowViewModel : RegionViewModelBase
    {
        public DelegateCommand SetSessionCommand { get; }

        public AnalyticsWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {

        }
    }
}
