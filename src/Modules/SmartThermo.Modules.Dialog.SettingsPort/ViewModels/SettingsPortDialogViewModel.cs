using Prism.Mvvm;
using Prism.Services.Dialogs;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Models;
using System;

namespace SmartThermo.Modules.Dialog.SettingsPort.ViewModels
{
    public class SettingsPortDialogViewModel : BindableBase, IDialogAware
    {
        private string _message;
        private readonly IDeviceConnector _deviceConnector;

        public event Action<IDialogResult> RequestClose;

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public string Title => null;

        public SettingsPortDialogViewModel( IDeviceConnector deviceConnector)
        {
            _deviceConnector = deviceConnector;

            UploadingDataSources();
            SetDefaultSettings();
        }

        private void SetDefaultSettings()
        {
            throw new NotImplementedException();
        }

        private void UploadingDataSources()
        {
            throw new NotImplementedException();
        }

        public void OnDialogClosed()
        {
            _deviceConnector.SettingPort = new SettingDevice
            {
                
            };
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }

        public bool CanCloseDialog() => true;
    }
}
