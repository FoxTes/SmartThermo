using System.Threading.Tasks;

namespace SmartThermo.Services.Configuration
{
    /// <summary>
    /// Представляет набор свойств конфигурации приложения.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Время, после которого датчик переходит в режим ожидания.
        /// </summary>
        public int TimeBeforeWarning { get; set; }

        /// <summary>
        /// Время, после которого связь с датчиком потеряна.
        /// </summary>
        public int TimeBeforeOffline { get; set; }

        /// <summary>
        /// Автоподключение к прибору.
        /// </summary>
        public bool IsAutoConnect { get; set; }

        /// <summary>
        /// Вести запись полученных данных.
        /// </summary>
        public bool IsWriteToDatabase { get; set; }

        /// <summary>
        /// Асинхронно сохраняет все изменения, внесенные в IConfiguration, в основную базу данных.
        /// </summary>
        Task SaveChangedAsync();
    }
}