using SmartThermo.DataAccess.Sqlite.Models.Base;
using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class Setting : BaseModel
    {
        public List<SelectMode> SelectModes { get; set; }

        public byte AddressDeviceSelected { get; set; }

        public string PortNameSelected { get; set; }

        public string BaudRateSelected { get; set; }
    }
}
