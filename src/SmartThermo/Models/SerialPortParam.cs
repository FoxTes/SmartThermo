using SmartThermo.Enums;
using System.IO.Ports;

namespace SmartThermo.Models
{
    public class SerialPortParam
    {
        public string Name { get; set; }

        public BaudRate BaudRate { get; set; }

        public Parity Parity { get; set; }

        public DataBits DataBits { get; set; }

        public StopBits StopBits { get; set; }
    }
}
