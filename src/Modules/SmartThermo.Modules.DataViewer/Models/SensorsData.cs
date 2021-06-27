using SmartThermo.Core.Enums;

namespace SmartThermo.Modules.DataViewer.Models
{
    public struct SensorsData
    {
        public int? Temperature { get; set; }
        
        public StatusSensor StatusSensor { get; set; }
    }
}