using System;

namespace SmartThermo.Modules.Analytics.Models
{
    public class SessionInfo
    {
        public int Id { get; set; }

        public DateTime DateCreate { get; set; }

        public int CountRecord { get; set; }

        public override string ToString()
        {
            return $"От {DateCreate} - {CountRecord} записи";
        }
    }
}
