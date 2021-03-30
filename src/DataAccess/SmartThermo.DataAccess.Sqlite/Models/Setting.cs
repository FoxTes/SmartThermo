using System.Collections.Generic;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class Setting
    {
        public int Id { get; set; }

        public List<SelectMode> SelectModes { get; set; }
    }
}
