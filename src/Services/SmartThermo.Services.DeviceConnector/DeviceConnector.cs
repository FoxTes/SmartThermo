using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;
using NModbus;
using NModbus.Serial;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;

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
        private readonly  ModbusFactory _modbusFactory;
        
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
        
        public void Open()
        {
            //_serialPort.PortName = SettingPort.NamePort;
            //_serialPort.BaudRate = SettingPort.BaudRate;
            _serialPort.Open();

            _serialPortAdapter = new SerialPortAdapter(_serialPort)
            {
                WriteTimeout = 500,
                ReadTimeout = 500
            };
            _modbusSerialMaster = _modbusFactory.CreateRtuMaster(_serialPortAdapter);

            StatusConnect = StatusConnect.Connected;
            StatusConnectChanged?.Invoke(this, StatusConnect);
        }

        public void Close()
        {
            _modbusSerialMaster?.Dispose();
            _serialPortAdapter?.Dispose();
            
            _serialPort.Close();
            
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