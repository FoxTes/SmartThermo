using System.IO.Ports;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class SettingDevice
    {
        public string NamePort { get; }
        
        public int BaudRate { get; }
        
        public Parity Parity { get; }
        
        public StopBits StopBits { get; }
        
        public int DataBits { get; }
        
        public Handshake Handshake { get; }
        
        public int WriteTimeout { get; }
        
        public int ReadTimeout { get; }
        
        public SettingDevice(Handshake handshake, int dataBits, StopBits stopBits, 
            Parity parity, int baudRate, string namePort, int writeTimeout, int readTimeout)
        {
            Handshake = handshake;
            DataBits = dataBits;
            StopBits = stopBits;
            Parity = parity;
            BaudRate = baudRate;
            NamePort = namePort;
            WriteTimeout = writeTimeout;
            ReadTimeout = readTimeout;
        }
    }
}