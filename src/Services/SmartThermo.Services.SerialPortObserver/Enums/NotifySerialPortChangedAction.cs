namespace SmartThermo.Services.SerialPortObserver.Enums
{
    /// <summary>
    /// Описывает действие, вызвавшее событие SerialPortChanged.
    /// </summary>
    public enum NotifySerialPortChangedAction
    {
        /// <summary>
        /// Добавлен последовательный порт.
        /// </summary>
        Add,

        /// <summary>
        /// Удален последовательный порт.
        /// </summary>
        Remove
    }
}