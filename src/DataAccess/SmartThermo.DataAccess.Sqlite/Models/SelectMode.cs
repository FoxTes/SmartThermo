namespace SmartThermo.DataAccess.Sqlite.Models
{
    public class SelectMode
    {
        public int Id { get; set; }

        public bool Stage { get; set; }

        public int SettingId { get; set; }

        public Setting Setting { get; set; }
    }
}
