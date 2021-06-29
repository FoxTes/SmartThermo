using System;
using SmartThermo.Services.SerialPortObserver.Models;

namespace SmartThermo.Services.SerialPortObserver
{
    /// <summary>
    /// Представляет сервис для отслеживания изменений состояния последовательного порта.
    /// </summary>
    public interface ISerialPortObserver
    {
        /// <summary>
        /// Указывает, что было событие, изменившие список имен последовательных портов.
        /// </summary>
        event EventHandler<SerialPortChangedArgs> SerialPortChanged;

        /// <summary>
        /// Запускает сканирование.
        /// </summary>
        void Start();

        /// <summary>
        /// Останавливает сканирование.
        /// </summary>
        void Stop();
    }
}