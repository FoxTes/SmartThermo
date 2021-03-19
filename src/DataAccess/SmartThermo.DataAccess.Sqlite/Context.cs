using Microsoft.EntityFrameworkCore;
using SmartThermo.DataAccess.Sqlite.Models;

namespace SmartThermo.DataAccess.Sqlite
{
    public class Context : DbContext
    {
        public DbSet<Session> Sessions { get; set; }

        public DbSet<SensorGroup> GroupSensors { get; set; }

        public DbSet<SensorInformation> SensorInformations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=app.db");

    }
}
