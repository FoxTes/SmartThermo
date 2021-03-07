using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SmartThermo.Modules.Dialog.SettingsPort.Enums;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using ToastNotifications.Core;

namespace SmartThermo.Modules.Dialog.SettingsPort.ViewModels
{
    public class SettingsPortDialogViewModel : BindableBase, IDialogAware
    {
        #region Event

        public event Action<IDialogResult> RequestClose;

        #endregion

        #region Field

        private readonly IDeviceConnector _deviceConnector;
        private readonly INotifications _notifications;

        private byte _addressDeviceSelected;
        private List<byte> _addressDevice;

        private string _portNameSelected;
        private string[] _portName;

        private BaudRate _baudRateSelected;
        private StopBits _stopBitsSelected;
        private Parity _paritySelected;

        private bool _isEnable = true;

        #endregion

        #region Propetry

        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }

        public string Title => null;

        public byte AddressDeviceSelected
        {
            get => _addressDeviceSelected;
            set => SetProperty(ref _addressDeviceSelected, value);
        }

        public List<byte> AddressDevice
        {
            get => _addressDevice;
            set => SetProperty(ref _addressDevice, value);
        }

        public string PortNameSelected
        {
            get => _portNameSelected;
            set => SetProperty(ref _portNameSelected, value);
        }

        public string[] PortName
        {
            get => _portName;
            set => SetProperty(ref _portName, value);
        }

        public BaudRate BaudRateSelected
        {
            get => _baudRateSelected;
            set => SetProperty(ref _baudRateSelected, value);
        }

        public StopBits StopBitsSelected
        {
            get => _stopBitsSelected;
            set => SetProperty(ref _stopBitsSelected, value);
        }

        public Parity ParitySelected
        {
            get => _paritySelected;
            set => SetProperty(ref _paritySelected, value);
        }

        public DelegateCommand ConnectCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion

        #region Constructor

        public SettingsPortDialogViewModel(IDeviceConnector deviceConnector, INotifications notifications)
        {
            _deviceConnector = deviceConnector;
            _notifications = notifications;

            UploadingDataSources();
            SetDefaultSettings();

            ConnectCommand = new DelegateCommand(ConnectExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
        }

        #endregion

        #region Method

        private async void ConnectExecute()
        {
            IsEnable = false;

            _deviceConnector.SettingPortPort = new SettingPortDevice()
            {
                AddressDevice = AddressDeviceSelected,
                NamePort = PortNameSelected,
                BaudRate = (int)BaudRateSelected,
                DataBits = 8,
                StopBits = StopBitsSelected,
                Parity = ParitySelected,
                ReadTimeout = 500,
                WriteTimeout = 500
            };
            try
            {
                await _deviceConnector.Open();
            }

            catch (TimeoutException)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Устройство не отвечает.", new MessageOptions());
            }

            catch (Exception ex)
            {
                _deviceConnector.Close(false);
                _notifications.ShowWarning("Не удалось открыть соединение.\n" + ex.Message, new MessageOptions());
            }
            finally
            {
                IsEnable = true;
            }

            RequestClose?.Invoke(new DialogResult(ButtonResult.None));
        }

        private void CancelExecute() => RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));

        private void UploadingDataSources()
        {
            AddressDevice = new List<byte>();
            for (byte i = 0; i < 255; i++)
                AddressDevice.Add(i);

            PortName = SerialPort.GetPortNames();
        }

        private void SetDefaultSettings()
        {
            AddressDeviceSelected = 10;
            PortNameSelected = PortName[0];
            BaudRateSelected = BaudRate.S9600;
            StopBitsSelected = StopBits.Two;
            ParitySelected = Parity.None;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

        }

        public bool CanCloseDialog() => true;

        #endregion
    }
}
