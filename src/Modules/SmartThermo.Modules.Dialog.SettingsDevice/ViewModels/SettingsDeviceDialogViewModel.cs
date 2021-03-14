﻿using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SmartThermo.Modules.Dialog.SettingsDevice.Models;
using SmartThermo.Modules.Dialog.SettingsPort.Enums;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.BitExtensions;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Threading.Tasks;
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

        private byte _addressDeviceInModbus;
        private BaudRate _baudRateSelected;
        private Parity _paritySelected;
        private byte _communicationChanelNumber;
        private byte _modemFrequencyChannelNumber;

        private List<ushort> _temperatureThreshold;
        private List<byte> _temperatureHysteresis;
        private List<byte> _delaySignalRelays;
        private List<bool> _statusAlarmRelay;

        private ObservableCollection<GroupInfo> _groupCheckItems = new ObservableCollection<GroupInfo>();
        private List<RelayNumber> _relayNumber;
        private int _relayNumberSelected;
        private bool _workLogic;

        #endregion

        #region Propetry

        public byte AddressDeviceInModbus
        {
            get => _addressDeviceInModbus;
            set => SetProperty(ref _addressDeviceInModbus, value);
        }

        public BaudRate BaudRateSelected
        {
            get => _baudRateSelected;
            set => SetProperty(ref _baudRateSelected, value);
        }

        public Parity ParitySelected
        {
            get => _paritySelected;
            set => SetProperty(ref _paritySelected, value);
        }

        public byte CommunicationChanelNumber
        {
            get => _communicationChanelNumber;
            set => SetProperty(ref _communicationChanelNumber, value);
        }

        public byte ModemFrequencyChannelNumber
        {
            get => _modemFrequencyChannelNumber;
            set => SetProperty(ref _modemFrequencyChannelNumber, value);
        }

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

        public ObservableCollection<GroupInfo> GroupCheckItems
        {
            get => _groupCheckItems;
            set => SetProperty(ref _groupCheckItems, value);
        }

        public List<RelayNumber> RelayNumber
        {
            get => _relayNumber;
            set => SetProperty(ref _relayNumber, value);
        }

        public int RelayNumberSelected
        {
            get => _relayNumberSelected;
            set
            {
                SetProperty(ref _relayNumberSelected, value);
                UpdateGroupCheckItems(value);
            }
        }

        public bool WorkLogic
        {
            get => _workLogic;
            set => SetProperty(ref _workLogic, value);
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

            RelayNumber = new List<RelayNumber>
            {
                new RelayNumber {Name = "Реле №1", Value = 1},
                new RelayNumber {Name = "Реле №2", Value = 2},
                new RelayNumber {Name = "Реле №3", Value = 3}
            };
            RelayNumberSelected = 1;

            WriteCommand = new DelegateCommand(WriteExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
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
                var setting = new SettingDeviceEventArgs
                {
                    TemperatureThreshold = TemperatureThreshold,
                    TemperatureHysteresis = (ushort)((TemperatureHysteresis[1] << 8) | TemperatureHysteresis[0]),
                    DelaySignalRelays = (ushort)((DelaySignalRelays[1] << 8) | DelaySignalRelays[0]),
                    StatusAlarmRelay = (ushort)((StatusAlarmRelay[2] ? 0x04 : 0x00) |
                                                 (StatusAlarmRelay[1] ? 0x02 : 0x00) |
                                                 (StatusAlarmRelay[0] ? 0x01 : 0x00))
                };
                await _deviceConnector.SetSettingDevice(setting);

                isSuccessful = true;
            }
            catch (TimeoutException)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства. Устройство не отвечает.",
                    new MessageOptions());
                _deviceConnector.Close();
            }
            catch (NotImplementedException)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства. Данный функционал еще не реализован.",
                    new MessageOptions());
                _deviceConnector.Close();
            }
            catch (NullReferenceException)
            {
                _notifications.ShowWarning("Отсутствует соединение с устройством.",
                    new MessageOptions());
            }
            catch (Exception ex)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства.\n" + ex.Message,
                    new MessageOptions());
                _deviceConnector.Close();
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

        private void UpdateGroupCheckItems(int value)
        {
            ushort data = value switch
            {
                1 => _deviceConnector.SettingDevice.BindingSensorRelay1,
                2 => _deviceConnector.SettingDevice.BindingSensorRelay2,
                3 => _deviceConnector.SettingDevice.BindingSensorRelay3,
                _ => 0
            };

            GroupCheckItems.Clear();
            for (var i = 0; i < 6; i++)
                GroupCheckItems.Add(new GroupInfo
                {
                    Name = $"Группа {i + 1} (датчики {(i + 1) * 10 + 1}-{(i + 1) * 10 + 7})",
                    Enable = data.IsBitSet(i)
                });

            WorkLogic = data.IsBitSet(7);
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            GetDeviceSettingsAsync();
        }

        private void GetDeviceSettingsAsync()
        {
            var setting =  _deviceConnector.SettingDevice;

            AddressDeviceInModbus = (byte) setting.AddressDevice;
            BaudRateSelected = setting.Speed switch
            {
                0 => BaudRate.S9600,
                1 => BaudRate.S19200,
                2 => BaudRate.S38400,
                3 => BaudRate.S57600,
                _ => BaudRate.S115200
            };
            ParitySelected = setting.Parity switch
            {
                0 => Parity.None,
                1 => Parity.Even,
                _ => Parity.Odd
            };
            CommunicationChanelNumber = (byte) setting.NumberChanelId;
            ModemFrequencyChannelNumber = (byte) (setting.NumberChanelId >> 8);

            TemperatureThreshold = setting.TemperatureThreshold;
            TemperatureHysteresis = new List<byte>
            {
                (byte) setting.TemperatureHysteresis,
                (byte) (setting.TemperatureHysteresis >> 8)
            };
            DelaySignalRelays = new List<byte>
            {
                (byte) setting.DelaySignalRelays,
                (byte) (setting.DelaySignalRelays >> 8)
            };
            StatusAlarmRelay = new List<bool>
            {
                setting.StatusAlarmRelay.IsBitSet(0),
                setting.StatusAlarmRelay.IsBitSet(1),
                setting.StatusAlarmRelay.IsBitSet(2)
            };
        }

        public bool CanCloseDialog() => true;

        #endregion
    }
}
