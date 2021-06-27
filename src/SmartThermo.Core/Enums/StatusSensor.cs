namespace SmartThermo.Core.Enums
{
    /// <summary>
    /// Статусное состояние датчиков.
    /// </summary>
    public enum StatusSensor
    {
        /// <summary>
        /// Датчик находится в сети.
        /// </summary>
        Online,

        /// <summary>
        /// Датчик не отвечает.
        /// </summary>
        Wait,

        /// <summary>
        /// Датчик находится не в сети.
        /// </summary>
        Offline
    }
}