using Microsoft.Extensions.Logging;
using NModbus;
using NModbus.Serial;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Helpers;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SmartThermo.Core.Extensions;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnector : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;
        public event EventHandler<SettingDeviceEventArgs> SettingDeviceChanged;

        #endregion

        #region Const

        private const int TimeStart = 1;
        private const int TimePeriod = 10;

        #endregion

        #region Field

        private readonly SerialPort _serialPort;
        private readonly ModbusFactory _modbusFactory;
        private readonly Timer _timer;
        private readonly INotifications _notifications;

        private SerialPortAdapter _serialPortAdapter;
        private IModbusSerialMaster _modbusSerialMaster;

        #endregion

        #region Property

        public StatusConnect StatusConnect { get; private set; }

        public SettingPortDevice SettingPortPort { get; set; }

        public SettingDeviceEventArgs SettingDevice { get; private set; }

        #endregion

        #region Constructor

        public DeviceConnector(INotifications notifications, ILogger logger)
        {
            _serialPort = new SerialPort();
            _modbusFactory = new ModbusFactory(logger: new ModbusSerilog(LoggingLevel.Trace, logger));
            _timer = new Timer(OnTimer, 0, Timeout.Infinite, Timeout.Infinite);

            _notifications = notifications;
            StatusConnect = StatusConnect.Disconnected;
        }

        #endregion

        #region Method

        public async Task Open()
        {
            _serialPort.PortName = SettingPortPort.NamePort;
            _serialPort.BaudRate = SettingPortPort.BaudRate;
            _serialPort.DataBits = SettingPortPort.DataBits;
            _serialPort.StopBits = SettingPortPort.StopBits;
            _serialPort.Parity = SettingPortPort.Parity;
            _serialPort.Open();

            _serialPortAdapter = new SerialPortAdapter(_serialPort)
            {
                WriteTimeout = SettingPortPort.WriteTimeout,
                ReadTimeout = SettingPortPort.ReadTimeout
            };
            _modbusSerialMaster = _modbusFactory.CreateRtuMaster(_serialPortAdapter);

            await GetSettingDevice();
            StartTimer();

            StatusConnect = StatusConnect.Connected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public void Close(bool notification = true)
        {
            _modbusSerialMaster?.Dispose();
            _serialPortAdapter?.Dispose();

            _serialPort.Close();
            StopTimer();

            if (!notification)
                return;
            StatusConnect = StatusConnect.Disconnected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public async Task GetSettingDevice()
        {
            const ushort startRegister = (ushort)RegisterAddress.AddressDevice;
            var countRegister =
                (ushort)(Math.Abs(startRegister - RegisterAddress.StatusAlarmRelay) + 1);

            var dataRead = await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPortPort.AddressDevice,
                startRegister, countRegister);

            SettingDevice = new SettingDeviceEventArgs()
            {
                AddressDevice = dataRead[0],
                Speed = dataRead[1],
                Parity = dataRead[2],
                NumberChanelId = dataRead[3],
                TemperatureThreshold = new List<ushort> { dataRead[4], dataRead[5] },
                TemperatureHysteresis = dataRead[6],
                DelaySignalRelays = dataRead[7],
                BindingSensorRelay1 = dataRead[8],
                BindingSensorRelay2 = dataRead[9],
                BindingSensorRelay3 = dataRead[10],
                StatusAlarmRelay = dataRead[11]
            };
        }

        public async Task SetSettingDevice(SettingDeviceEventArgs settingDevice)
        {
            var data = new[]
            {
                settingDevice.AddressDevice,
                settingDevice.Speed,
                settingDevice.Parity,
                settingDevice.NumberChanelId,
                settingDevice.TemperatureThreshold[0],
                settingDevice.TemperatureThreshold[1],
                settingDevice.TemperatureHysteresis,
                settingDevice.DelaySignalRelays,
                settingDevice.BindingSensorRelay1,
                settingDevice.BindingSensorRelay2,
                settingDevice.BindingSensorRelay3,
                settingDevice.StatusAlarmRelay
            };

            const ushort startRegister = (ushort)RegisterAddress.AddressDevice;
            await _modbusSerialMaster.WriteMultipleRegistersAsync(SettingPortPort.AddressDevice,
                startRegister, data);

            SettingDevice = settingDevice;
            SettingDeviceChanged?.Invoke(this, settingDevice);
        }

        private async void OnTimer(object state)
        {
            const ushort startRegister = (ushort)RegisterAddress.Sensor11;
            const ushort countRegister = 36;

            var dataRead = new ushort[countRegister];
            try
            {
                dataRead = await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPortPort.AddressDevice,
                    startRegister, countRegister);
            }
            catch (TimeoutException)
            {
                Close();
                _notifications.ShowWarning("Устройство не отвечает на чтение регистров.");
            }
            catch (Exception ex)
            {
                Close();
                _notifications.ShowWarning("Не удалось прочитать регистры.\n" + ex.Message);
            }

            var result = dataRead.Select((x, index) => new SensorInfoEventArgs
            {
                Id = index,
                Number = (index / 6 + 1) * 10 + (index + 1) - 6 * (index / 6),
                Temperature = (byte)dataRead[index],
                TimeLastBroadcast = (byte)((dataRead[index] & 0b0011_1111_0000_0000) >> 8),
                IsEmergencyDescent = dataRead[index].IsBitSet(14),
                IsAir = dataRead[index].IsBitSet(15)
            }).ToList();
            RegistersRequested?.Invoke(this, result);
        }

        private void StartTimer() 
            => _timer.Change(TimeSpan.FromSeconds(TimeStart), TimeSpan.FromSeconds(TimePeriod));

        private void StopTimer() 
            => _timer.Change(Timeout.Infinite, Timeout.Infinite);

        #endregion
    }
}