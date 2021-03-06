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

        public event EventHandler<List<double>> RegistersRequested;
        public event EventHandler<StatusConnect> StatusConnectChanged;

        #endregion

        #region Field

        private readonly SerialPort _serialPort;
        private readonly ModbusFactory _modbusFactory;

        private SerialPortAdapter _serialPortAdapter;
        private IModbusSerialMaster _modbusSerialMaster;

        #endregion

        #region Property

        public StatusConnect StatusConnect { get; private set; }
        public SettingDevice SettingPort { get; set; }

        #endregion

        public DeviceConnector()
        {
            _serialPort = new SerialPort();
            _modbusFactory = new ModbusFactory();

            StatusConnect = StatusConnect.Disconnected;
        }

        public async Task Open()
        {
            _serialPort.PortName = SettingPort.NamePort;
            _serialPort.BaudRate = SettingPort.BaudRate;
            _serialPort.DataBits = SettingPort.DataBits;
            _serialPort.StopBits = SettingPort.StopBits;
            _serialPort.Parity = SettingPort.Parity;
            _serialPort.Open();

            _serialPortAdapter = new SerialPortAdapter(_serialPort)
            {
                WriteTimeout = SettingPort.WriteTimeout,
                ReadTimeout = SettingPort.ReadTimeout
            };
            _modbusSerialMaster = _modbusFactory.CreateRtuMaster(_serialPortAdapter);
            await _modbusSerialMaster.ReadHoldingRegistersAsync(SettingPort.AddressDevice, (ushort)RegisterAddress.FirmwareVersion, 1);

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

        public Task<List<LimitTrigger>> GetLimitTriggerDevice()
        {
            throw new NotImplementedException();
        }

        public Task SetLimitTriggerDevice(List<LimitTrigger> limitTriggers)
        {
            throw new NotImplementedException();
        }
    }
}