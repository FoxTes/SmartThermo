using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using SmartThermo.Services.DeviceConnector.BitExtensions;
using SmartThermo.Services.DeviceConnector.Models;
using ToastNotifications.Core;

namespace SmartThermo.Modules.Dialog.SettingsDevice.ViewModels
{
    public class SettingsDeviceDialogViewModel : BindableBase, IDialogAware
    {
        #region Event

        public event Action<IDialogResult> RequestClose;

        #endregion

        #region Field

        private readonly IDeviceConnector _deviceConnector;
        private readonly INotifications _notifications;

        private bool _isEnable = true;
        private bool _isSettingsSet;

        private List<ushort> _temperatureThreshold;
        private List<byte> _temperatureHysteresis;
        private List<byte> _delaySignalRelays;
        private List<bool> _statusAlarmRelay;

        #endregion

        #region Propetry

        public List<ushort> TemperatureThreshold
        {
            get => _temperatureThreshold;
            set => SetProperty(ref _temperatureThreshold, value);
        }
        
        public List<byte> TemperatureHysteresis
        {
            get => _temperatureHysteresis;
            set => SetProperty(ref _temperatureHysteresis, value);
        }
        
        public List<byte> DelaySignalRelays
        {
            get => _delaySignalRelays;
            set => SetProperty(ref _delaySignalRelays, value);
        }

        public List<bool> StatusAlarmRelay
        {
            get => _statusAlarmRelay;
            set => SetProperty(ref _statusAlarmRelay, value);
        }
        
        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }
        
        public string Title => string.Empty;

        public DelegateCommand WriteCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion

        #region Constructor

        public SettingsDeviceDialogViewModel(IDeviceConnector deviceConnector, INotifications notifications)
        {
            _deviceConnector = deviceConnector;
            _notifications = notifications;

            WriteCommand = new DelegateCommand(WriteExecute);
            CancelCommand = new DelegateCommand(CancelExecute);

            TemperatureThreshold = new List<ushort>() {1, 2};
        }

        #endregion

        #region Method

        private async void WriteExecute()
        {
            _isSettingsSet = true;
            IsEnable = false;
            
            var isSuccessful = false;
            try
            {
                var setting = new SettingDevice
                {
                    TemperatureThreshold = TemperatureThreshold,
                    TemperatureHysteresis = (ushort) ((TemperatureHysteresis[1] << 8) | TemperatureHysteresis[0]),
                    DelaySignalRelays = (ushort) ((DelaySignalRelays[1] << 8) | DelaySignalRelays[0]),
                    StatusAlarmRelay = (ushort) ((StatusAlarmRelay[2] ? 0x04 : 0x00) |
                                                 (StatusAlarmRelay[1] ? 0x02 : 0x00) |
                                                 (StatusAlarmRelay[0] ? 0x01 : 0x00))
                };
                await _deviceConnector.SetSettingDevice(setting);

                isSuccessful = true;
            }
            catch (TimeoutException)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось записать настройки устройства. Устройство не отвечает.",
                    new MessageOptions());
            }
            catch (NullReferenceException)
            {
                _notifications.ShowWarning("Отсутствует соединение с устройством.",
                    new MessageOptions());
            }
            catch (Exception ex)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось записать настройки устройства.\n" + ex.Message, 
                    new MessageOptions());
            }
            finally
            {
                IsEnable = true;
            }

            if (!isSuccessful)
                RequestClose?.Invoke(new DialogResult(ButtonResult.None));
        }

        private void CancelExecute()
        {
            RequestClose?.Invoke(new DialogResult(_isSettingsSet ? ButtonResult.None : ButtonResult.Cancel));
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            GetDeviceSettingsAsync();
        }

        private async void GetDeviceSettingsAsync()
        {
            IsEnable = false;

            var isSuccessful = false;
            try
            {
                var setting = await _deviceConnector.GetSettingDevice();
                
                TemperatureThreshold = setting.TemperatureThreshold;
                TemperatureHysteresis = new List<byte>
                {
                    (byte)setting.TemperatureHysteresis, 
                    (byte)(setting.TemperatureHysteresis >> 8)
                };
                DelaySignalRelays = new List<byte>
                {
                    (byte)setting.DelaySignalRelays,
                    (byte)(setting.DelaySignalRelays >> 8)
                };
                StatusAlarmRelay = new List<bool>
                {
                    setting.StatusAlarmRelay.IsBitSet(0),
                    setting.StatusAlarmRelay.IsBitSet(1),
                    setting.StatusAlarmRelay.IsBitSet(2),
                };

                isSuccessful = true;
            }
            catch (TimeoutException)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось считать настройки устройства. Устройство не отвечает.", 
                    new MessageOptions());
            }
            catch (Exception ex)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось считать настройки устройства.\n" + ex.Message, 
                    new MessageOptions());
            }
            finally
            {
                IsEnable = true;
            }

            if (!isSuccessful)
                RequestClose?.Invoke(new DialogResult(ButtonResult.None));
        }

        public bool CanCloseDialog() => true;

        #endregion
    }
}
