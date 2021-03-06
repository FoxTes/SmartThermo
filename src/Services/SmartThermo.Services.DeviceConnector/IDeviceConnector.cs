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
        event EventHandler<List<double>> RegistersRequested;
        
        /// <summary>
        /// Событие, возникающие при изменение состоения подключения к прибору.
        /// </summary>
        event EventHandler<StatusConnect> StatusConnectChanged;
        
        /// <summary>
        /// Получает значение, указывающее состоение соединения с прибором.
        /// </summary>
        StatusConnect StatusConnect { get; }
        
        /// <summary>
        /// Вовзращает или задает настройки порта.
        /// </summary>
        SettingDevice SettingPort { get; set; }
        
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
        /// Получает настройки прибора.
        /// </summary>
        /// <returns></returns>
        Task<List<LimitTrigger>> GetLimitTriggerDevice();
        
        /// <summary>
        /// Устанавливает настройки прибора.
        /// </summary>
        /// <returns></returns>
        Task SetLimitTriggerDevice(List<LimitTrigger> limitTriggers);
    }
}