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
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;

namespace SmartThermo.ViewModels
{
    public class MainWindowViewModel : RegionViewModelBase
    {
        #region Field

        private bool _isEnableSettings;
        private string _labelButton = "Подключить прибор";
        private string _labelView = "Измерительный участок";

        #endregion

        #region Property

        public bool IsEnableSettings
        {
            get => _isEnableSettings;
            set => SetProperty(ref _isEnableSettings, value);
        }

        public string LabelButton
        {
            get => _labelButton;
            set => SetProperty(ref _labelButton, value);
        }

        public string LabelView
        {
            get => _labelView;
            set => SetProperty(ref _labelView, value);
        }

        public DelegateCommand ChangeConnectDeviceCommand { get; }

        public DelegateCommand SettingDeviceCommand { get; }

        public DelegateCommand<NavigationViewItemInvokedEventArgs> NavigationViewInvokedCommand { get; }

        public DelegateCommand AboutCommand { get; }

        #endregion

        #region Constructor

        public MainWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications, IDialogService dialogService)
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            DeviceConnector.StatusConnectChanged += async (_, connect) =>
            {
                if (DeviceConnector.StatusConnect == StatusConnect.Connected)
                {
                    IsEnableSettings = true;
                    LabelButton = "Отключить прибор";

                    await Task.Delay(250);
                    Notifications.ShowSuccess("Осуществлено подключение к прибору.");
                }
                else
                {
                    IsEnableSettings = false;
                    LabelButton = "Подключить прибор";

                    await Task.Delay(250);
                    Notifications.ShowInformation("Осуществлено отключение от прибора.");
                }
            };

            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
            SettingDeviceCommand = new DelegateCommand(SettingDeviceExecute);
            NavigationViewInvokedCommand = new DelegateCommand<NavigationViewItemInvokedEventArgs>(NavigationViewInvokedExecute);
            AboutCommand = new DelegateCommand(AboutExecute);

            CreateSession();
        }

        #endregion

        #region Method

        private async void CreateSession()
        {
            CheckDatabaseCreate();

            await using var context = new Context();
            try
            {
                context.Add(new Session
                {
                    DateCreate = DateTime.Now,
                    SensorGroups = new List<SensorGroup>
                    {
                        new SensorGroup { Name = "Первая группа"},
                        new SensorGroup { Name = "Вторая группа"},
                        new SensorGroup { Name = "Третья группа"},
                        new SensorGroup { Name = "Четвертая группа"},
                        new SensorGroup { Name = "Пятая группа"},
                        new SensorGroup { Name = "Шестая группа"},
                    }
                });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                await Task.Delay(1000);
                Notifications.ShowError("Внимание! Ошибка работы с БД. Приложение будет закрыто через 3 секунды.");
                await Task.Delay(3000);

                Application.Current.Shutdown();
            }
        }

        private static void CheckDatabaseCreate()
        {
            using var context = new Context();
            context.Database.EnsureCreated();
        }

        private void ChangeConnectDeviceExecute()
        {
            if (DeviceConnector.StatusConnect == StatusConnect.Connected)
                try
                {
                    DeviceConnector.Close();
                }
                catch (Exception ex)
                {
                    Notifications.ShowWarning("Не удалось закрыть соединение.\n" + ex.Message);
                }
            else
            {
                if (SerialPort.GetPortNames().Length == 0)
                {
                    Notifications.ShowError("В компьютере найдены активные COM порты.");
                    return;
                }
                DialogService.ShowNotification("SettingsPortDialog", r =>
                {
                    if (r.Result == ButtonResult.Cancel)
                        Notifications.ShowInformation("Операция прервана пользователем.");
                }, windowName: "NotificationWindow");
            }

        }

        private void SettingDeviceExecute()
        {
            DialogService.ShowNotification("SettingsDeviceDialog", r =>
            {
                if (r.Result == ButtonResult.Cancel)
                    Notifications.ShowInformation("Операция прервана пользователем.");
            }, windowName: "NotificationWindow");
        }

        private static void AboutExecute()
        {
            var dialog = new AboutDialog();
            dialog.ShowAsync();
        }

        private void NavigationViewInvokedExecute(NavigationViewItemInvokedEventArgs obj)
        {
            LabelView = obj.InvokedItem.ToString();

            var nameRegion = obj.InvokedItemContainer.Tag.ToString();
            RegionManager.RequestNavigate(RegionNames.MainContent, nameRegion);
        }

        #endregion
    }
}
