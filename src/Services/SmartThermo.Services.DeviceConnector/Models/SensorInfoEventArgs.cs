using System;

namespace SmartThermo.Services.DeviceConnector.Models
{
    public class SensorInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Порядковый номер.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Температура [0..155] в градусах Цельсия.
        /// </summary>
        public byte Temperature { get; set; }

        /// <summary>
        /// Время прошедшее с последнего сеанса связи [0...63сек].
        /// </summary>
        public byte TimeLastBroadcast { get; set; }

        /// <summary>
        /// Флаг аварийного снижения напряжения питания при низком токе шины [1 - питание аварийно снижается, 0 - питание в норме].
        /// </summary>
        public bool IsEmergencyDescent { get; set; }

        /// <summary>
        /// Флаг присутствия датчика в эфире [1 - присутствует, 0 - отсутствует].
        /// </summary>
        public bool IsAir { get; set; }
    }
}
