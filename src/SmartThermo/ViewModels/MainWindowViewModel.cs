using System;
using Prism.Commands;
using Prism.Regions;
using SmartThermo.Core.Mvvm;
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

        public MainWindowViewModel(IRegionManager regionManager, IDeviceConnector deviceConnector, INotifications notifications) 
            : base(regionManager, deviceConnector, notifications)
        {
            DeviceConnector.StatusConnectChanged += (_, connect) =>
            {
                if (DeviceConnector.StatusConnect == StatusConnect.Connected)
                {
                    LabelButton = "Отключить прибор";
                    Notifications.ShowSuccess("Осуществленно подключение к прибору.");
                }
                else
                {
                    LabelButton = "Подключить прибор";
                    Notifications.ShowInformation("Осуществленно отключение от прибора.");
                }
            };
            
            ChangeConnectDeviceCommand = new DelegateCommand(ChangeConnectDeviceExecute);
        }
        
        #endregion

        #region Method

        private void ChangeConnectDeviceExecute()
        {
            if (DeviceConnector.StatusConnect == StatusConnect.Connected)
            {
                try
                {
                    DeviceConnector.Close();
                }
                catch (Exception ex)
                {
                    Notifications.ShowWarning("Не удалось закрыть соединение.\n" + ex.Message, new MessageOptions());
                }
            }
            else
            {
                try
                {
                    DeviceConnector.Open();
                }
                catch (Exception ex)
                {
                    Notifications.ShowWarning("Не удалось открыть соединение.\n" + ex.Message, new MessageOptions());
                }
            }
        }
        
        #endregion
    }
}
