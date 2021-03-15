namespace SmartThermo.Modules.DataViewer.Models
{
    public class LimitRelay
    {
        public double TemperatureThreshold1 { get; set; }

        public double HysteresisThreshold1 { get; set; }

        public double TemperatureThreshold2 { get; set; }

        public double HysteresisThreshold2 { get; set; }
    }
}
