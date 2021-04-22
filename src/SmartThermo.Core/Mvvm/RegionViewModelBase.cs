using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System;

namespace SmartThermo.Core.Mvvm
{
    public class RegionViewModelBase : ViewModelBase, IConfirmNavigationRequest
    {
        protected IRegionManager RegionManager { get; }
        protected IDialogService DialogService { get; }
        protected IDeviceConnector DeviceConnector { get; }
        protected INotifications Notifications { get; }

        protected RegionViewModelBase(IRegionManager regionManager, IDeviceConnector deviceConnector, 
            INotifications notifications, IDialogService dialogService)
        {
            RegionManager = regionManager;
            DeviceConnector = deviceConnector;
            Notifications = notifications;
            DialogService = dialogService;
        }

        public virtual void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            continuationCallback(true);
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
