using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartThermo.Services.DeviceConnector.Enums;
using SmartThermo.Services.DeviceConnector.Models;

namespace SmartThermo.Services.DeviceConnector
{
    public interface IDeviceConnector
    {
        /// <summary>
        /// Событие, возникающие при опросе регистров.
        /// </summary>
        event EventHandler<List<SensorInfoEventArgs>> RegistersRequested;
        
        /// <summary>
        /// Событие, возникающие при изменение состояние подключения к прибору.
        /// </summary>
        event EventHandler<StatusConnect> StatusConnectChanged;

        /// <summary>
        /// Событие, возникающие при настроек прибора.
        /// </summary>
        event EventHandler<SettingDeviceEventArgs> SettingDeviceChanged;

        /// <summary>
        /// Получает значение, указывающее состояние соединения с прибором.
        /// </summary>
        StatusConnect StatusConnect { get; }
        
        /// <summary>
        /// Возвращает или задает настройки порта.
        /// </summary>
        SettingPortDevice SettingPortPort { get; set; }

        /// <summary>
        /// Возвращает или задает настройки порта.
        /// </summary>
        SettingDeviceEventArgs SettingDevice { get; }

        /// <summary>
        /// Открывает новое соединение с прибором.
        /// </summary>
        /// <returns></returns>
        Task Open();

        /// <summary>
        /// Закрывает соединение порта и уничтожает внутренний объект Stream.
        /// </summary>
        /// <param name="notification">Разрешает уведомление о закрытии.</param>
        void Close(bool notification = true);

        /// <summary>
        /// Получает общие настройки прибора.
        /// </summary>
        /// <returns></returns>
        Task<SettingDeviceEventArgs> GetSettingDevice();

        /// <summary>
        /// Получает общие настройки прибора.
        /// </summary>
        /// <returns></returns>
        Task SetSettingDevice(SettingDeviceEventArgs settingDevice);
    }
}