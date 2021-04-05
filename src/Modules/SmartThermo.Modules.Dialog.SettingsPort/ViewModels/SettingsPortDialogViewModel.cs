using Prism.Commands;
using Prism.Services.Dialogs;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.Dialog.SettingsPort.Enums;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace SmartThermo.Modules.Dialog.SettingsPort.ViewModels
{
    public class SettingsPortDialogViewModel : DialogViewModelBase
    {
        #region Field

        private readonly IDeviceConnector _deviceConnector;
        private readonly INotifications _notifications;

        private byte _addressDeviceSelected;
        private List<byte> _addressDevice;

        private string _portNameSelected;
        private ReadOnlyCollection<string> _portName;

        private BaudRate _baudRateSelected;
        private StopBits _stopBitsSelected;
        private Parity _paritySelected;
        private double _readTimeout;
        private double _writeTimeout;

        private bool _isEnable = true;

        #endregion

        #region Propetry

        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }

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

        public ReadOnlyCollection<string> PortName
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

        public double ReadTimeout
        {
            get => _readTimeout;
            set => SetProperty(ref _readTimeout, value);
        }

        public double WriteTimeout
        {
            get => _writeTimeout;
            set => SetProperty(ref _writeTimeout, value);
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
                ReadTimeout = ReadTimeout.Equals(double.NaN) ? 100 : (int)ReadTimeout,
                WriteTimeout = WriteTimeout.Equals(double.NaN) ? 100 : (int)WriteTimeout
            };
            try
            {
                await _deviceConnector.Open();
            }
            catch (TimeoutException)
            {
                _notifications.ShowWarning("Не удалось считать настройки устройства. Устройство не отвечает.");
                _deviceConnector.Close(false);
            }
            catch (Exception ex)
            {
                _notifications.ShowWarning("Не удалось открыть соединение.\n" + ex.Message);
                _deviceConnector.Close(false);
            }
            finally
            {
                IsEnable = true;
            }

            RaiseRequestClose(new DialogResult(ButtonResult.None));
        }

        private void CancelExecute() => RaiseRequestClose(new DialogResult(ButtonResult.Cancel));

        private void UploadingDataSources()
        {
            AddressDevice = new List<byte>();
            for (byte i = 0; i < 255; i++)
                AddressDevice.Add(i);

            PortName = new ReadOnlyCollection<string>(SerialPort.GetPortNames());
        }

        private void SetDefaultSettings()
        {
            PortNameSelected = PortName[0];
            AddressDeviceSelected = 3;
            BaudRateSelected = BaudRate.S9600;
            StopBitsSelected = StopBits.One;
            ParitySelected = Parity.None;
            ReadTimeout = 250;
            WriteTimeout = 250;
        }

        #endregion
    }
}
