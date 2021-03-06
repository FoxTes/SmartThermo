using System;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
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
        
        #endregion

        #region Property
        
        public string LabelButton
        {
            get => _labelButton;
            set => SetProperty(ref _labelButton, value);
        }
        
        public DelegateCommand ChangeConnectDeviceCommand { get; }
        
        #endregion

        #region Constructor

        public MainWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications, IDialogService dialogService) 
            : base(regionManager, deviceConnector, notifications, dialogService)
        {
            DeviceConnector.StatusConnectChanged += (_, connect) =>
            {
                if (DeviceConnector.StatusConnect == StatusConnect.Connected)
                {
                    LabelButton = "Отключить прибор";
                    Notifications.ShowSuccess("Осуществлено подключение к прибору.");
                }
                else
                {
                    LabelButton = "Подключить прибор";
                    Notifications.ShowInformation("Осуществлено отключение от прибора.");
                }
            };
            
            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
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
                DialogService.ShowNotification(string.Empty, r =>
                {
                    if (r.Result == ButtonResult.Cancel)
                        Notifications.ShowInformation("Операция прервана пользователем.");
                });
        }
        
        #endregion
    }
}
