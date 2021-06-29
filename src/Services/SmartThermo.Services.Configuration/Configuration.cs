using System.Linq;
using System.Threading.Tasks;
using SmartThermo.DataAccess.Sqlite;

namespace SmartThermo.Services.Configuration
{
    /// <inheritdoc />
    public class Configuration : IConfiguration
    {
        /// <summary>
        /// Инициализирует значения.
        /// </summary>
        public Configuration()
        {
            FillValue();
        }

        /// <inheritdoc />
        public int TimeBeforeWarning { get; set; }

        /// <inheritdoc />
        public int TimeBeforeOffline { get; set; }

        /// <inheritdoc />
        public bool IsAutoConnect { get; set; }

        /// <inheritdoc />
        public bool IsWriteToDatabase { get; set; }

        /// <inheritdoc />
        public async Task SaveChangedAsync()
        {
            var saveDataTask = Task.Run(() =>
            {
                using var context = new Context();

                var data = context.Settings.First();
                data.TimeBeforeWarning = TimeBeforeWarning;
                data.TimeBeforeOffline = TimeBeforeOffline;
                data.IsAutoConnect = IsAutoConnect;
                data.IsWriteToDatabase = IsWriteToDatabase;

                return context.SaveChanges();
            });
            await Task.WhenAll(saveDataTask);
        }

        private void FillValue()
        {
            using var context = new Context();
            var data = context.Settings.First();

            TimeBeforeWarning = data.TimeBeforeWarning;
            TimeBeforeOffline = data.TimeBeforeOffline;
            IsAutoConnect = data.IsAutoConnect;
            IsWriteToDatabase = data.IsWriteToDatabase;
        }
    }
}