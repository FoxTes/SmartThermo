using System.Collections.Generic;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class SettingDevice
    {
        /// <summary>
        /// Температурный порог №1 в градусах Цельсия (00..07) [0 - 155].
        /// </summary>
        public List<ushort> TemperatureThreshold { get; set; }

        /// <summary>
        /// Температурный гистерезис в градусах Цельсия (00..07) (08..15) [0-100].
        /// </summary>
        public ushort TemperatureHysteresis { get; set; }

        /// <summary>
        /// Задержка включения/выключения сигнальных реле в секундах (00..07) (08..15) [0-255].
        /// </summary>
        public ushort DelaySignalRelays { get; set; }

        /// <summary>
        /// Статус сигнальных реле (00..00) (01..01) (02..02) [0 - выкл, 1 - вкл].
        /// </summary>
        public ushort StatusAlarmRelay { get; set; }
    }
}
