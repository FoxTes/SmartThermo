using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class GroupSensor
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Session Session { get; set; }

        public ICollection<SensorInfo> SensorInfos { get; set; }
    }
}
