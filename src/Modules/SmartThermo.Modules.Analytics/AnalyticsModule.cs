using Prism.Ioc;
using Prism.Modularity;
using SmartThermo.Modules.Analytics.Dialogs.ViewModels;
using SmartThermo.Modules.Analytics.Dialogs.Views;
using SmartThermo.Modules.Analytics.ViewModels;
using SmartThermo.Modules.Analytics.Views;

namespace SmartThermo.Modules.Analytics
{
    public class AnalyticsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SessionDialog, SessionDialogViewModel>();
            containerRegistry.RegisterForNavigation<AnalyticsWindow, AnalyticsWindowViewModel>();
        }
    }
}