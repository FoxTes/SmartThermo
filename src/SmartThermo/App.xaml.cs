using System;
using Prism.Ioc;
using Prism.Modularity;
using SmartThermo.Modules.DataViewer;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using SmartThermo.Views;
using System.Windows;
using SmartThermo.Modules.Dialog.SettingsDevice.ViewModels;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;
using SmartThermo.Modules.Dialog.SettingsPort.Views;
using SmartThermo.Modules.Dialog.SettingsPort.ViewModels;
using SmartThermo.Modules.Dialog.SettingsDevice.Views;

namespace SmartThermo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var instance = new Notifications(new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(Current.MainWindow, 
                    Corner.BottomRight, 25, 0);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(3),
                    MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Current.Dispatcher;
                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            }));

            containerRegistry.RegisterInstance<INotifications>(instance);
            #if DEBUG
                containerRegistry.RegisterSingleton<IDeviceConnector, DeviceConnectorTest>();
            #else
                containerRegistry.RegisterSingleton<IDeviceConnector, DeviceConnector>();
            #endif

            containerRegistry.RegisterDialog<SettingsPortDialog, SettingsPortDialogViewModel>();
            containerRegistry.RegisterDialog<SettingsDeviceDialog, SettingsDeviceDialogViewModel>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<DataViewerModule>();
        }

        // TODO: 5) Далее с аналитикой. 6) Венеси в ресурсы стиль Dialog окон. 7) Сделать базовую модель для Dialog окон.
    }
}