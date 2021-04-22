using Prism.Ioc;
using Prism.Modularity;
using SmartThermo.Modules.Settings.ViewModels;
using SmartThermo.Modules.Settings.Views;

namespace SmartThermo.Modules.Settings
{
    public class SettingsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsWindow, SettingsWindowViewModel>();
        }
    }
}