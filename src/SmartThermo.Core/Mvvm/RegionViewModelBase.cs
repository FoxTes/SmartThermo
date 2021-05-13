using Prism.Regions;
using System;

namespace SmartThermo.Core.Mvvm
{
    /// <summary>
    /// ViewModel предоставляющая возможность методов навигации.
    /// </summary>
    public class RegionViewModelBase : ViewModelBase, IConfirmNavigationRequest
    {
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
