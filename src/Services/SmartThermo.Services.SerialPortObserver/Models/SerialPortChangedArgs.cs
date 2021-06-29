using SmartThermo.Services.SerialPortObserver.Enums;

namespace SmartThermo.Services.SerialPortObserver.Models
{
    /// <summary>
    /// Предоставляет данные для события SerialPortChanged.
    /// </summary>
    public class SerialPortChangedArgs
    {
        /// <summary>
        /// Инициализирует новый экземпляр класса SerialPortChangedArgs,
        /// который описывает событие вызвавшее событие, и список новых имен последовательных портов.
        /// </summary>
        /// <param name="notifySerialPortChangedAction">Действие, вызвавшее событие.</param>
        /// <param name="serialPorts">Список новых имен последовательных портов.</param>
        public SerialPortChangedArgs(NotifySerialPortChangedAction notifySerialPortChangedAction, string[] serialPorts)
        {
            NotifySerialPortChangedAction = notifySerialPortChangedAction;
            SerialPorts = serialPorts;
        }

        /// <summary>
        /// Получает список новых имен последовательных портов.
        /// </summary>
        public string[] SerialPorts { get; }

        /// <summary>
        /// Получает действие, вызвавшее событие.
        /// </summary>
        public NotifySerialPortChangedAction NotifySerialPortChangedAction { get; }
    }
}