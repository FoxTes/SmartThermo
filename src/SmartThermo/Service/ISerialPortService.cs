using SmartThermo.Enums;
using SmartThermo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartThermo.Service
{
    public interface ISerialPortService
    {
        event EventHandler<StatusConnect> StatusConnectChanged;

        string PortName { get; }

        void Open(SerialPortParam serialPortParam);

        void Close();

        IEnumerable<string> GetPortNames();

        void WriteData(ushort address);

        ushort[] ReadData(ushort address);
    }
}
