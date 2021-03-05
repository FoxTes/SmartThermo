using System.IO.Ports;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class SettingDevice
    {
        public string NamePort { get; set; }
        
        public int BaudRate { get; set; }
        
        public Parity Parity { get; set; }

        public StopBits StopBits { get; set; }

        public int DataBits { get; set; }
        
        public Handshake Handshake { get; set; }

        public int WriteTimeout { get; set; }
        
        public int ReadTimeout { get; set; }
    }
}