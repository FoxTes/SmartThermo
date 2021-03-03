using System;
using Prism.Regions;
using SmartThermo.Services.DeviceConnector;

namespace SmartThermo.Core.Mvvm
{
    public class RegionViewModelBase : ViewModelBase, IConfirmNavigationRequest
    {
        protected IRegionManager RegionManager { get; }
        protected IDeviceConnector DeviceConnector { get; }

        public RegionViewModelBase(IRegionManager regionManager, IDeviceConnector deviceConnector)
        {
            RegionManager = regionManager;
            DeviceConnector = deviceConnector;
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
