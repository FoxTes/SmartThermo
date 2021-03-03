using Prism.Regions;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System;

namespace SmartThermo.Core.Mvvm
{
    public class RegionViewModelBase : ViewModelBase, IConfirmNavigationRequest
    {
        protected IRegionManager RegionManager { get; }
        protected IDeviceConnector DeviceConnector { get; }
        protected INotifications Notifications { get; }

        public RegionViewModelBase(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications)
        {
            RegionManager = regionManager;
            DeviceConnector = deviceConnector;
            Notifications = notifications;
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
