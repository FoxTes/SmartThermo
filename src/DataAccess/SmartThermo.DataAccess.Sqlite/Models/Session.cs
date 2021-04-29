using SmartThermo.DataAccess.Sqlite.Models.Base;
using System;
using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class Session : BaseModel
    {
        public DateTime DateCreate { get; set; }

        public List<SensorGroup> SensorGroups { get; set; }
    }
}
