using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using SmartThermo.Core;
using SmartThermo.Modules.SettingsApplication.ViewModels;
using SmartThermo.Modules.SettingsApplication.Views;

namespace SmartThermo.Modules.SettingsApplication
{
    /// <summary>
    /// Модуль для настроек приложения.
    /// </summary>
    public class SettingsApplicationModule : IModule
    {
        private readonly IRegionManager _regionManager;

        /// <summary>
        /// Инициализация модуля настроек приложения.
        /// </summary>
        /// <param name="regionManager">Менеджер регионов.</param>
        public SettingsApplicationModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        /// <inheritdoc />
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        /// <inheritdoc />
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SettingsApplicationWindow, SettingsApplicationWindowViewModel>();
        }
    }
}