using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using ModernWpf.Controls;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core;
using SmartThermo.Core.Extensions;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.DataAccess.Sqlite.Models;
using SmartThermo.Dialogs.Views;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.Notifications;

namespace SmartThermo.ViewModels
{
    /// <inheritdoc />
    public class MainWindowViewModel : RegionViewModelBase
    {
        private readonly IDeviceConnector _deviceConnector;
        private readonly IDialogService _dialogService;
        private readonly IRegionManager _regionManager;
        private readonly INotifications _notifications;

        private bool _isEnableSettings;
        private string _labelButton = "Подключить прибор";
        private string _labelView = "Измерительный участок";

        /// <inheritdoc />
        public MainWindowViewModel(
            IRegionManager regionManager,
            IDeviceConnector deviceConnector,
            INotifications notifications,
            IDialogService dialogService)
        {
            _deviceConnector = deviceConnector;
            _dialogService = dialogService;
            _notifications = notifications;
            _regionManager = regionManager;

            _deviceConnector.StatusConnectChanged += OnDeviceConnectorOnStatusConnectChanged;

            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
            SettingDeviceCommand = new DelegateCommand(SettingDeviceExecute)
                .ObservesCanExecute(() => IsEnableSettings);
            NavigationViewInvokedCommand
                = new DelegateCommand<NavigationViewItemInvokedEventArgs>(NavigationViewInvokedExecute);
            SettingSensorCommand = new DelegateCommand(SettingSensorExecute);
            AboutCommand = new DelegateCommand(AboutExecute);

            CreateSession();
        }

        /// <summary>
        /// Название кнопки.
        /// </summary>
        public string LabelButton
        {
            get => _labelButton;
            set => SetProperty(ref _labelButton, value);
        }

        /// <summary>
        /// Название представления.
        /// </summary>
        public string LabelView
        {
            get => _labelView;
            set => SetProperty(ref _labelView, value);
        }

        /// <summary>
        /// Команда для изменения статуса подключения.
        /// </summary>
        public DelegateCommand ChangeConnectDeviceCommand { get; }

        /// <summary>
        /// Команда для вызова настроек устройства.
        /// </summary>
        public DelegateCommand SettingDeviceCommand { get; }

        /// <summary>
        /// Команда для изменения представления.
        /// </summary>
        public DelegateCommand<NavigationViewItemInvokedEventArgs> NavigationViewInvokedCommand { get; }

        /// <summary>
        /// Команда для вызова настроек датчиков.
        /// </summary>
        public DelegateCommand SettingSensorCommand { get; }

        /// <summary>
        /// Команда для вызова окна "о программе".
        /// </summary>
        public DelegateCommand AboutCommand { get; }

        private bool IsEnableSettings
        {
            get => _isEnableSettings;
            set => SetProperty(ref _isEnableSettings, value);
        }
        
        private static void AboutExecute()
        {
            var dialog = new AboutDialog();
            dialog.ShowAsync();
        }

        private async void OnDeviceConnectorOnStatusConnectChanged(object x, StatusConnect connect)
        {
            if (_deviceConnector.StatusConnect == StatusConnect.Connected)
            {
                IsEnableSettings = true;
                LabelButton = "Отключить прибор";

                await Task.Delay(250);
                _notifications.ShowSuccess("Осуществлено подключение к прибору.");
            }
            else
            {
                IsEnableSettings = false;
                LabelButton = "Подключить прибор";

                await Task.Delay(250);
                _notifications.ShowInformation("Осуществлено отключение от прибора.");
            }
        }

        private async void CreateSession()
        {
            try
            {
                await using var context = new Context();
                context.Add(new Session
                {
                    DateCreate = DateTime.Now,
                    SensorGroups = new List<SensorGroup>
                    {
                        new SensorGroup { Name = "Первая группа" },
                        new SensorGroup { Name = "Вторая группа" },
                        new SensorGroup { Name = "Третья группа" },
                        new SensorGroup { Name = "Четвертая группа" },
                        new SensorGroup { Name = "Пятая группа" },
                        new SensorGroup { Name = "Шестая группа" }
                    }
                });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                _notifications.ShowError("Внимание! Ошибка работы с БД. Приложение будет закрыто через 3 секунды.");
                await Task.Delay(3000);

                Application.Current.Shutdown();
            }
        }

        private void ChangeConnectDeviceExecute()
        {
            if (_deviceConnector.StatusConnect == StatusConnect.Connected)
            {
                try
                {
                    _deviceConnector.Close();
                }
                catch (Exception ex)
                {
                    _notifications.ShowWarning("Не удалось закрыть соединение.\n" + ex.Message);
                }
            }
            else
            {
                if (SerialPort.GetPortNames().Length == 0)
                {
                    _notifications.ShowError("В компьютере не найдены активные COM порты.");
                    return;
                }

                _dialogService.ShowNotification(
                    "SettingsPortDialog",
                    r =>
                    {
                        if (r.Result == ButtonResult.Cancel)
                            _notifications.ShowInformation("Операция прервана пользователем.");
                    },
                    windowName: "NotificationWindow");
            }
        }

        private void SettingDeviceExecute()
        {
            _dialogService.ShowNotification(
                "SettingsDeviceDialog",
                r =>
                {
                    if (r.Result == ButtonResult.Cancel)
                        _notifications.ShowInformation("Операция прервана пользователем.");
                },
                windowName: "NotificationWindowCloseButton");
        }

        private void SettingSensorExecute()
        {
            if (SerialPort.GetPortNames().Length == 0)
            {
                _notifications.ShowError("В компьютере не найдены активные COM порты.");
                return;
            }

            _dialogService.ShowNotification(
                "SettingsSensorDialog",
                r => { },
                windowName: "NotificationWindowCloseButton");
        }

        private void NavigationViewInvokedExecute(NavigationViewItemInvokedEventArgs obj)
        {
            LabelView = obj.InvokedItem.ToString();
            var nameRegion = obj.InvokedItemContainer.Tag.ToString();
            _regionManager.RequestNavigate(RegionNames.MainContent,
                obj.IsSettingsInvoked ? "SettingsApplicationWindow" : nameRegion);
        }
    }
}
