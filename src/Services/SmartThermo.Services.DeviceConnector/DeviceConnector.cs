using NModbus;
using NModbus.Serial;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SmartThermo.Services.DeviceConnector
{
    public class DeviceConnector : IDeviceConnector
    {
        #region Event

        public event EventHandler<StatusConnect> StatusConnectChanged;
        public event EventHandler<List<SensorInfo>> RegistersRequested;

        #endregion

        #region Field

        private readonly SerialPort _serialPort;
        private readonly ModbusFactory _modbusFactory;

        private SerialPortAdapter _serialPortAdapter;
        private IModbusSerialMaster _modbusSerialMaster;

        #endregion

        #region Property

        public StatusConnect StatusConnect { get; private set; }
        public SettingPortDevice SettingPortPort { get; set; }

        #endregion

        #region Constructor

        public DeviceConnector()
        {
            _serialPort = new SerialPort();
            _modbusFactory = new ModbusFactory();

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
                (ushort) RegisterAddress.FirmwareVersion, 1);

            StatusConnect = StatusConnect.Connected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public void Close(bool notification = true)
        {
            _modbusSerialMaster?.Dispose();
            _serialPortAdapter?.Dispose();

            _serialPort.Close();

            if (!notification)
                return;
            StatusConnect = StatusConnect.Disconnected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public async Task<SettingDevice> GetSettingDevice()
        {
            const ushort startRegister = (ushort)RegisterAddress.TemperatureThreshold1;
            var countRegister =
                (ushort) (Math.Abs(RegisterAddress.TemperatureThreshold1 - RegisterAddress.StatusAlarmRelay) + 1);
            
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

        public Task<List<LimitTrigger>> GetLimitTriggerDevice()
        {
            throw new NotImplementedException();
        }

        public Task SetLimitTriggerDevice(List<LimitTrigger> limitTriggers)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}