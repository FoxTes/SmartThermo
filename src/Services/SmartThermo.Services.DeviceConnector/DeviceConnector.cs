using NModbus;
using NModbus.Serial;
using SmartThermo.Services.DeviceConnector.BitExtensions;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using SmartThermo.Services.Notifications;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToastNotifications.Core;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnector : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;

        #endregion

        #region Const

        private const int TimeStart = 1;
        private const int TimePeriod = 3;

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

        #endregion

        #region Constructor

        public DeviceConnector(INotifications notifications)
        {
            _serialPort = new SerialPort();
            _modbusFactory = new ModbusFactory();
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
            await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPortPort.AddressDevice,
                (ushort)RegisterAddress.FirmwareVersion, 1);

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

        public async Task<SettingDevice> GetSettingDevice()
        {
            const ushort startRegister = (ushort)RegisterAddress.TemperatureThreshold1;
            var countRegister =
                (ushort)(Math.Abs(RegisterAddress.TemperatureThreshold1 - RegisterAddress.StatusAlarmRelay) + 1);

            var data = await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPortPort.AddressDevice,
                startRegister, countRegister);

            return new SettingDevice()
            {
                TemperatureThreshold = new List<ushort> { data[0], data[1] },
                TemperatureHysteresis = data[3],
                DelaySignalRelays = data[4],
                StatusAlarmRelay = data[7]
            };
        }

        public async Task SetSettingDevice(SettingDevice settingDevice)
        {
            const ushort startRegister = (ushort)RegisterAddress.TemperatureThreshold1;
            var data = new[]
            {
                settingDevice.TemperatureThreshold[0],
                settingDevice.TemperatureThreshold[1],
                settingDevice.TemperatureHysteresis,
                settingDevice.DelaySignalRelays
            };

            await _modbusSerialMaster.WriteMultipleRegistersAsync(SettingPortPort.AddressDevice,
                startRegister, data);
            await _modbusSerialMaster.WriteSingleRegisterAsync(SettingPortPort.AddressDevice,
                (ushort)RegisterAddress.StatusAlarmRelay, settingDevice.StatusAlarmRelay);
        }

        public Task<List<LimitTriggerEventArgs>> GetLimitTriggerDevice()
        {
            throw new NotImplementedException();
        }

        public Task SetLimitTriggerDevice(List<LimitTriggerEventArgs> limitTriggers)
        {
            throw new NotImplementedException();
        }

        private async void OnTimer(object state)
        {
            const ushort startRegister = (ushort)RegisterAddress.Sensor11;
            const ushort countRegister = 36;

            var data = new ushort[countRegister];
            try
            {
                data = await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPortPort.AddressDevice,
                    startRegister, countRegister);
            }
            catch (TimeoutException)
            {
                Close();
                _notifications.ShowWarning("Устройство не отвечает на чтение регистров.", new MessageOptions());
            }
            catch (Exception ex)
            {
                Close();
                _notifications.ShowWarning("Не удалось прочитать регистры.\n" + ex.Message, new MessageOptions());
            }

            var result = data.Select((x , index)=> new SensorInfoEventArgs
            {
                Id = index + 1,
                Temperature = (byte)data[index],
                TimeLastBroadcast = (byte)((data[index] & 0b0011_1111_0000_0000) >> 8),
                IsEmergencyDescent = data[index].IsBitSet(14),
                IsAir = data[index].IsBitSet(15)
            }).ToList();
            RegistersRequested?.Invoke(this, result);
        }

        private void StartTimer()
        {
            _timer.Change(TimeSpan.FromSeconds(TimeStart), TimeSpan.FromSeconds(TimePeriod));
        }

        private void StopTimer()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        #endregion
    }
}