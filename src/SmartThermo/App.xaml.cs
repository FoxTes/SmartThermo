using System;
using System.Globalization;
using System.Text;
using System.Windows;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.Dialogs.Views;
using SmartThermo.Modules.Analytics;
using SmartThermo.Modules.DataViewer;
using SmartThermo.Modules.Dialog.SettingsDevice.ViewModels;
using SmartThermo.Modules.Dialog.SettingsDevice.Views;
using SmartThermo.Modules.Dialog.SettingsPort.ViewModels;
using SmartThermo.Modules.Dialog.SettingsPort.Views;
using SmartThermo.Modules.Dialog.SettingsSensor.ViewModels;
using SmartThermo.Modules.Dialog.SettingsSensor.Views;
using SmartThermo.Modules.SettingsApplication;
using SmartThermo.Services.Configuration;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using SmartThermo.Services.SerialPortObserver;
using SmartThermo.Views;
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
        /// <inheritdoc />
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e)
                => MessageBox.Show(e.ExceptionObject.ToString());
        }

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            CheckDatabaseCreate();
            SetCountryCode();
            AppCenter.Start("3fb4a695-2ae6-4663-9878-d0fa3ada2d1e",
                typeof(Analytics),
                typeof(Crashes));

            base.OnStartup(e);
        }

        /// <inheritdoc />
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <inheritdoc />
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            var serilogLogger = Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log\\logModbus.log",
                    encoding: Encoding.UTF8,
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File("log\\logError.log",
                    encoding: Encoding.UTF8,
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();
            var appLogger = new SerilogLoggerProvider(serilogLogger).CreateLogger("App");
            containerRegistry.RegisterInstance(appLogger);

            var instance = new Services.Notifications.Notifications(new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(Current.MainWindow, Corner.BottomRight, 16, 16);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    TimeSpan.FromSeconds(3),
                    MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Current.Dispatcher;
                cfg.DisplayOptions.TopMost = true;
                cfg.DisplayOptions.Width = 250;
            }));

            containerRegistry.RegisterInstance<INotifications>(instance);
            containerRegistry.RegisterSingleton<IConfiguration, Configuration>();
            containerRegistry.RegisterSingleton<ISerialPortObserver, SerialPortObserver>();
#if DEBUG
            containerRegistry.RegisterSingleton<IDeviceConnector, DeviceConnectorTest>();
#else
            containerRegistry.RegisterSingleton<IDeviceConnector, DeviceConnector>();
#endif
            containerRegistry.RegisterDialog<SettingsPortDialog, SettingsPortDialogViewModel>();
            containerRegistry.RegisterDialog<SettingsDeviceDialog, SettingsDeviceDialogViewModel>();
            containerRegistry.RegisterDialog<SettingsSensorDialog, SettingsSensorDialogViewModel>();

            containerRegistry.RegisterDialogWindow<NotificationWindow>("NotificationWindow");
            containerRegistry.RegisterDialogWindow<NotificationWindowCloseButton>("NotificationWindowCloseButton");
        }

        /// <inheritdoc />
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<DataViewerModule>();
            moduleCatalog.AddModule<AnalyticsModule>();
            moduleCatalog.AddModule<SettingsApplicationModule>();
        }

        private static void SetCountryCode()
        {
            var countryCode = RegionInfo.CurrentRegion.TwoLetterISORegionName;
            AppCenter.SetCountryCode(countryCode);
        }

        private static void CheckDatabaseCreate()
        {
            using var context = new Context();
            context.Database.EnsureCreated();
        }
    }
}