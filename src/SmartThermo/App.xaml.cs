using Prism.Ioc;
using Prism.Modularity;
using SmartThermo.Modules.Analytics;
using SmartThermo.Modules.DataViewer;
using SmartThermo.Modules.Dialog.SettingsDevice.ViewModels;
using SmartThermo.Modules.Dialog.SettingsDevice.Views;
using SmartThermo.Modules.Dialog.SettingsPort.ViewModels;
using SmartThermo.Modules.Dialog.SettingsPort.Views;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using SmartThermo.Views;
using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Position;

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
            moduleCatalog.AddModule<AnalyticsModule>();
        }

        // -Вынести стили диалоговых окон.
        // -Загрузка данных для Analytics.
        // -Переделка дизайна для Analytics и диалогового окна.
        // -Сброс масштаба графиков.
        // -Tooltip для графиков.
        // -Перенос базового класса диалоговых окон.
    }
}