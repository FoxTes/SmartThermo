using System;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class SensorInformation
    {
        public int Id { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Value3 { get; set; }

        public int Value4 { get; set; }

        public int Value5 { get; set; }

        public int Value6 { get; set; }

        public DateTime DataTime { get; set; }

        public int SensorGroupId { get; set; }

        public SensorGroup SensorGroup { get; set; }
    }
}
