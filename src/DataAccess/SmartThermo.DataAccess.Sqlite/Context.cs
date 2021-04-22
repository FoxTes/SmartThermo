using Microsoft.EntityFrameworkCore;
using SmartThermo.DataAccess.Sqlite.Models;

namespace SmartThermo.DataAccess.Sqlite
{
    public class Context : DbContext
    {
        public DbSet<Session> Sessions { get; set; }

        public DbSet<SensorGroup> GroupSensors { get; set; }

        public DbSet<SensorInformation> SensorInformations { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<SelectMode> SelectModes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Setting>().HasData(new Setting { Id = 1 });

            modelBuilder.Entity<SelectMode>().HasData(
                new SelectMode { Id = 1, SettingId = 1, Stage = false },
                new SelectMode { Id = 2, SettingId = 1, Stage = false },
                new SelectMode { Id = 3, SettingId = 1, Stage = false },
                new SelectMode { Id = 4, SettingId = 1, Stage = false },
                new SelectMode { Id = 5, SettingId = 1, Stage = false },
                new SelectMode { Id = 6, SettingId = 1, Stage = false });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=app.db");
    }
}
