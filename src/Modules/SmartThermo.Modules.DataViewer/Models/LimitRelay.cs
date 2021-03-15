namespace SmartThermo.Modules.DataViewer.Models
{
    public class LimitRelay
    {
        public byte TemperatureThreshold1 { get; set; }

        public byte HysteresisThreshold1 { get; set; }

        public byte TemperatureThreshold2 { get; set; }

        public byte HysteresisThreshold2 { get; set; }
    }
}
