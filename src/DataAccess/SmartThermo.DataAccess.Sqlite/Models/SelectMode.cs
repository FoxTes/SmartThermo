using SmartThermo.DataAccess.Sqlite.Models.Base;

namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class SelectMode : BaseModel
    {
        public bool Stage { get; set; }

        public int SettingId { get; set; }

        public Setting Setting { get; set; }
    }
}
