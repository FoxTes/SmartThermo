using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class Setting
    {
        public int Id { get; set; }

        public List<SelectMode> SelectModes { get; set; }

        public byte AddressDeviceSelected { get; set; }

        public string PortNameSelected { get; set; }

        public string BaudRateSelected { get; set; }
    }
}
