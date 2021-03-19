using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class SensorGroup
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int SessionId { get; set; }

        public Session Session { get; set; }

        public List<SensorInformation> SensorInformations { get; set; }
    }
}
