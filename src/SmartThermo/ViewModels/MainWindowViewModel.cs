﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core;
using SmartThermo.Core.Mvvm;
using SmartThermo.DataAccess.Sqlite;
using SmartThermo.DataAccess.Sqlite.Models;
using SmartThermo.DialogExtensions;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.Notifications;
using ToastNotifications.Core;

namespace SmartThermo.ViewModels
{
    public class MainWindowViewModel : RegionViewModelBase
    {
        #region Field

        private string _labelButton = "Подключить прибор";
        private bool _isEnableSettings;

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
        
        public DelegateCommand ChangeConnectDeviceCommand { get; }

        public DelegateCommand SettingDeviceCommand { get; }

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
                    regionManager.RequestNavigate(RegionNames.MainContent, "DataViewerWindow");

                    await Task.Delay(250);
                    Notifications.ShowSuccess("Осуществлено подключение к прибору.");
                }
                else
                {
                    IsEnableSettings = false;
                    LabelButton = "Подключить прибор";
                    regionManager.RequestNavigate(RegionNames.MainContent, "NoLoadDataViewerWindow");

                    await Task.Delay(250);
                    Notifications.ShowInformation("Осуществлено отключение от прибора.");
                }
            };
            
            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
            SettingDeviceCommand = new DelegateCommand(SettingDeviceExecute);

            using Context context = new Context();
            var result = context.Database.EnsureCreated();
        }

        #endregion

        #region Method

        private void ChangeConnectDeviceExecute()
        {
            if (DeviceConnector.StatusConnect == StatusConnect.Connected)
                try
                {
                    DeviceConnector.Close();
                }
                catch (Exception ex)
                {
                    Notifications.ShowWarning("Не удалось закрыть соединение.\n" + ex.Message, new MessageOptions());
                }
            else
                DialogService.ShowNotification("SettingsPortDialog", r =>
                {
                    if (r.Result == ButtonResult.Cancel)
                        Notifications.ShowInformation("Операция прервана пользователем.");
                });
        }

        private void SettingDeviceExecute()
        {
            DialogService.ShowNotification("SettingsDeviceDialog", r =>
            {
                if (r.Result == ButtonResult.Cancel)
                    Notifications.ShowInformation("Операция прервана пользователем.");
            });
        }

        #endregion
    }
}
