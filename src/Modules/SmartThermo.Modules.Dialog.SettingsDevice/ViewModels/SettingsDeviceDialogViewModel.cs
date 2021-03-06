﻿using Prism.Commands;
using Prism.Services.Dialogs;
using SmartThermo.Core.Extensions;
using SmartThermo.Core.Mvvm;
using SmartThermo.Modules.Dialog.SettingsDevice.Enums;
using SmartThermo.Modules.Dialog.SettingsDevice.Models;
using SmartThermo.Services.DeviceConnector;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using SmartThermo.Core.Enums;
using SmartThermo.Core.Models;
using Microsoft.Extensions.Logging;

namespace SmartThermo.Modules.Dialog.SettingsDevice.ViewModels
{
    public class SettingsDeviceDialogViewModel : DialogViewModelBase
    {
        #region Field

        private readonly ILogger _logger;
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

        private readonly List<ushort> _dataGroupCheckItems = new List<ushort>();
        private ObservableCollectionExtension<GroupInfo> _groupCheckItems = new ObservableCollectionExtension<GroupInfo>();
        private List<ItemDescriptor<int>> _relayNumber;
        private int _relayNumberSelected;
        private bool _workLogic;
        private BindingRelay _bindingRelayMode;
        private ObservableCollection<bool> _enableRelayMode = new ObservableCollection<bool>();

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

        public ObservableCollectionExtension<GroupInfo> GroupCheckItems
        {
            get => _groupCheckItems;
            set => SetProperty(ref _groupCheckItems, value);
        }

        public List<ItemDescriptor<int>> RelayNumber
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
            set
            {
                SetProperty(ref _workLogic, value);
                SetGroupCheckItems();
            }
        }

        public BindingRelay BindingRelayMode
        {
            get => _bindingRelayMode;
            set
            {
                SetProperty(ref _bindingRelayMode, value);
                SetGroupCheckItems();
            }
        }

        public ObservableCollection<bool> EnableRelayMode
        {
            get => _enableRelayMode;
            set => SetProperty(ref _enableRelayMode, value);
        }

        public bool IsEnable
        {
            get => _isEnable;
            set => SetProperty(ref _isEnable, value);
        }

        public DelegateCommand WriteCommand { get; }

        public DelegateCommand CancelCommand { get; }

        #endregion

        #region Constructor

        public SettingsDeviceDialogViewModel(IDeviceConnector deviceConnector, INotifications notifications, ILogger logger)
        {
            _deviceConnector = deviceConnector;
            _notifications = notifications;
            _logger = logger;

            _dataGroupCheckItems.Add(_deviceConnector.SettingDevice.BindingSensorRelay1);
            _dataGroupCheckItems.Add(_deviceConnector.SettingDevice.BindingSensorRelay2);
            _dataGroupCheckItems.Add(_deviceConnector.SettingDevice.BindingSensorRelay3);

            for (var i = 0; i < 6; i++)
                GroupCheckItems.Add(new GroupInfo
                {
                    Name = $"Группа {i + 1} (датчики {(i + 1) * 10 + 1}-{(i + 1) * 10 + 7})"
                });

            for (var i = 0; i < 3; i++)
                EnableRelayMode.Add(true);

            RelayNumber = new List<ItemDescriptor<int>>
            {
                new ItemDescriptor<int> ("Реле №1", 1),
                new ItemDescriptor<int> ("Реле №2", 2),
                new ItemDescriptor<int> ("Реле №3", 3)
            };
            RelayNumberSelected = 1;

            WriteCommand = new DelegateCommand(WriteExecute);
            CancelCommand = new DelegateCommand(CancelExecute);
            GroupCheckItems.CollectionChanged += (sender, args) => SetGroupCheckItems();
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
                    AddressDevice = AddressDeviceInModbus,
                    Speed = BaudRateSelected switch
                    {
                        BaudRate.S9600 => 0,
                        BaudRate.S19200 => 1,
                        BaudRate.S38400 => 2,
                        BaudRate.S57600 => 3,
                        _ => 4
                    },
                    Parity = ParitySelected switch
                    {
                        Parity.None => 0,
                        Parity.Even => 1,
                        _ => 2
                    },
                    NumberChanelId = (ushort)((CommunicationChanelNumber << 8) | ModemFrequencyChannelNumber),
                    TemperatureThreshold = TemperatureThreshold,
                    TemperatureHysteresis = (ushort)((TemperatureHysteresis[1] << 8) | TemperatureHysteresis[0]),
                    DelaySignalRelays = (ushort)((DelaySignalRelays[1] << 8) | DelaySignalRelays[0]),
                    BindingSensorRelay1 = _dataGroupCheckItems[0],
                    BindingSensorRelay2 = _dataGroupCheckItems[1],
                    BindingSensorRelay3 = _dataGroupCheckItems[2],
                    StatusAlarmRelay = (ushort)((StatusAlarmRelay[2] ? 0x04 : 0x00) |
                                                 (StatusAlarmRelay[1] ? 0x02 : 0x00) |
                                                 (StatusAlarmRelay[0] ? 0x01 : 0x00))
                };
                await _deviceConnector.SetSettingDevice(setting);

                _notifications.ShowInformation("Настройки успешно записаны.");
                isSuccessful = true;
            }
            catch (TimeoutException ex)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства. Устройство не отвечает.");
                _logger.LogError("{@Ex}\n", ex);
                _deviceConnector.Close();
            }
            catch (NotImplementedException)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства. Данный функционал еще не реализован.");
                _deviceConnector.Close();
            }
            catch (NullReferenceException)
            {
                _notifications.ShowWarning("Отсутствует соединение с устройством.");
            }
            catch (Exception ex)
            {
                _notifications.ShowWarning("Не удалось записать настройки устройства.\n" + ex.Message);
                _logger.LogError("{@Ex}\n", ex);
                _deviceConnector.Close();
            }
            finally
            {
                IsEnable = true;
            }

            if (!isSuccessful)
                RaiseRequestClose(new DialogResult(ButtonResult.None));
        }

        private void CancelExecute()
        {
            RaiseRequestClose(new DialogResult(_isSettingsSet ? ButtonResult.None : ButtonResult.Cancel));
        }

        private void UpdateGroupCheckItems(int value)
        {
            var modesSelected = _dataGroupCheckItems
                    .Select(x => (BindingRelay)((x & 0b0000_0011_0000_0000) >> 8))
                    .ToList();

            EnableRelayMode[0] = modesSelected.All(x => x != BindingRelay.СommunicationChanelFailure);
            EnableRelayMode[1] = modesSelected.All(x => x != BindingRelay.TemperatureThreshold1);
            EnableRelayMode[2] = modesSelected.All(x => x != BindingRelay.TemperatureThreshold2);

            var data = _dataGroupCheckItems[value - 1];

            for (var i = 0; i < 6; i++)
                GroupCheckItems[i].Enable = data.IsBitSet(i);
            WorkLogic = data.IsBitSet(7);
            BindingRelayMode = (BindingRelay)((data & 0b0000_0011_0000_0000) >> 8);

            switch (_bindingRelayMode)
            {
                case BindingRelay.СommunicationChanelFailure:
                    EnableRelayMode[0] = true;
                    break;
                case BindingRelay.TemperatureThreshold1:
                    EnableRelayMode[1] = true;
                    break;
                case BindingRelay.TemperatureThreshold2:
                    EnableRelayMode[2] = true;
                    break;
            }
            RaisePropertyChanged(nameof(EnableRelayMode));
        }

        private void SetGroupCheckItems()
        {
            ushort data = 0;

            for (var i = 0; i < _groupCheckItems.Count; i++)
                data |= _groupCheckItems[i].Enable ? (ushort)(1 << i) : (ushort)0x00;
            data |= _workLogic ? (ushort)0x80 : (ushort)0x00;
            data |= (ushort)((int)_bindingRelayMode << 8);

            _dataGroupCheckItems[_relayNumberSelected - 1] = data;
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            GetDeviceSettingsAsync();
        }

        private void GetDeviceSettingsAsync()
        {
            var setting = _deviceConnector.SettingDevice;

            AddressDeviceInModbus = (byte)setting.AddressDevice;
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
            ModemFrequencyChannelNumber = (byte)setting.NumberChanelId;
            CommunicationChanelNumber = (byte)(setting.NumberChanelId >> 8);

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

        #endregion
    }
}
