using NModbus;
using NModbus.Serial;
using SmartThermo.Enums;
using SmartThermo.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading.Tasks;

namespace SmartThermo.Service
{
    public class SerialPortService : ISerialPortService
    {
        public event EventHandler<StatusConnect> StatusConnectChanged;

        private readonly SerialPort _serialPort; 
        private IModbusMaster _modbusMaster;

        public string PortName => _serialPort.PortName;

        public SerialPortService()
        {
            _serialPort = new SerialPort();
        }

        public void Close()
        {
            _serialPort.Close();

            StatusConnectChanged?.Invoke(this, StatusConnect.Close);
        }

        public IEnumerable<string> GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void Open(SerialPortParam serialPortParam)
        {
            _serialPort.PortName = serialPortParam.Name;
            _serialPort.BaudRate = (int)serialPortParam.BaudRate;
            _serialPort.Parity = serialPortParam.Parity;
            _serialPort.DataBits = (int)serialPortParam.DataBits;
            _serialPort.StopBits = serialPortParam.StopBits;
            _serialPort.Open();


            var adapter = new SerialPortAdapter(_serialPort);
            adapter.WriteTimeout = 1000;
            adapter.ReadTimeout = 1000;

            // Create the factory
            var factory = new ModbusFactory();

            // Create Modbus Master
            _modbusMaster = factory.CreateRtuMaster(adapter);

            StatusConnectChanged?.Invoke(this, StatusConnect.Open);
        }

        public ushort[] ReadData(ushort address)
        {
            return _modbusMaster?.ReadHoldingRegisters(3, 0x00, 1);
        }

        public void WriteData(ushort address)
        {
            const byte slaveId = 3;
            const ushort startAddress = 1;
            var registers = new ushort[] { 1 };

            // write three registers
            _modbusMaster?.WriteMultipleRegisters(slaveId, startAddress, registers);
        }
    }
}
