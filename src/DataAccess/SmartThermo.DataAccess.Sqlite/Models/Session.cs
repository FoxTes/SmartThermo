using System;
using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class Session
    {
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        public List<SensorGroup> SensorGroups { get; set; }
    }
}
