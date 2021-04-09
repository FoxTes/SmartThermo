using System.IO.Ports;

namespace SmartThermo.Modules.Dialog.SettingsSensor.Models
{
    public class SettingPortSensor
    {
        public string NamePort { get; set; }

        public int BaudRate { get; set; }

        public StopBits StopBits { get; set; }
    }
}
