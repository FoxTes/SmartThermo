using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
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

        #endregion

        #region Propetry

        public List<ushort> TemperatureThreshold
        {
            get => _temperatureThreshold;
            set => SetProperty(ref _temperatureThreshold, value);
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

        private void WriteExecute()
        {
            _isSettingsSet = true;
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

                isSuccessful = true;
            }
            catch (TimeoutException)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Устройство не отвечает.", new MessageOptions());
            }
            catch (Exception ex)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось считать настройки устройства.\n" + ex.Message, new MessageOptions());
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
