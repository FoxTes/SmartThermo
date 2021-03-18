using System;
using System.Collections.Generic;
using System.Text;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class SensorInfo
    {
        public int Id { get; set; }

        public int Value1 { get; set; }

        public int Value2 { get; set; }

        public int Value3 { get; set; }

        public int Value4 { get; set; }

        public int Value5 { get; set; }

        public int Value6 { get; set; }

        public GroupSensor GroupSensor { get; set; }
    }
}
