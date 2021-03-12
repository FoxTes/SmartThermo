using System;
using System.Collections.Generic;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class SettingDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Адрес прибора в сети MODBUS [0-247].
        /// </summary>
        public ushort AddressDevice { get; set; }

        /// <summary>
        /// Скорость обмена [0 - 9600, 1 - 19200, 2 - 38400, 3 - 57600, 4 - 115200].
        /// </summary>
        public ushort Speed { get; set; }

        /// <summary>
        /// Паритет [0 - без проверки(два стоп - бита), 1 - четность(even)(один стоп-бит), 2 - нечетность(odd)(один стоп-бит)].
        /// </summary>
        public ushort Parite { get; set; }

        /// <summary>
        /// Номер канала связи и ID. 00..07 номер частотного канала модема [1-126] 08..15 ID[0..255].
        /// </summary>
        public ushort NumberChanelld { get; set; }

        /// <summary>
        /// Температурный порог в градусах Цельсия (00..07) [0 - 155].
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
        /// Привязка датчиков к реле 1.
        /// </summary>
        public ushort BindingSensorRelay1 { get; set; }

        /// <summary>
        /// Привязка датчиков к реле 2.
        /// </summary>
        public ushort BindingSensorRelay2 { get; set; }

        /// <summary>
        /// Привязка датчиков к реле 3.
        /// </summary>
        public ushort BindingSensorRelay3 { get; set; }

        /// <summary>
        /// Статус сигнальных реле (00..00) (01..01) (02..02) [0 - выкл, 1 - вкл].
        /// </summary>
        public ushort StatusAlarmRelay { get; set; }
    }
}
