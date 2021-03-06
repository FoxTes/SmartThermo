﻿namespace SmartThermo.Services.DeviceConnector.Enums
{
    /// <summary>
    /// Указывает адрес регистров, имеющийся в приборе.
    /// </summary>
    public enum RegisterAddress
    {
        /// <summary>
        /// Адрес устройства в сети Modbus.
        /// </summary>
        AddressDevice = 0,

        /// <summary>
        /// Скорость устройства для порта.
        /// </summary>
        Speed = 1,

        /// <summary>
        /// Настройки честности устройства.
        /// </summary>
        Parity = 2,

        /// <summary>
        /// Номер канала связи и ID.
        /// </summary>
        NumberCommunicationChannel = 3,

        /// <summary>
        /// Температурный порог №1.
        /// </summary>
        TemperatureThreshold1 = 4,

        /// <summary>
        /// Температурный порог №2.
        /// </summary>
        TemperatureThreshold2 = 5,

        /// <summary>
        /// Температурный гистерезис.
        /// </summary>
        TemperatureHysteresis = 6,

        /// <summary>
        /// Задержка включения/выключения сигнальных реле.
        /// </summary>
        DelaySignalRelays = 7,

        /// <summary>
        /// Статус сигнальных реле.
        /// </summary>
        StatusAlarmRelay = 11,

        /// <summary>
        /// Версия прошивки ПО.
        /// </summary>
        FirmwareVersion = 12,

        /// <summary>
        /// Бинарный код значения температуры, измеряемой датчиком №11.
        /// </summary>
        Sensor11 = 16,
    }
}