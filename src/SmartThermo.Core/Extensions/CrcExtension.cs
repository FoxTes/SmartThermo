using System.Collections.Generic;
using System.Linq;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Предоставляет методы для расчета Crc.
    /// </summary>
    public static class CrcExtension
    {
        /// <summary>
        /// Производит расчет Crc на основе сложения входной коллекции.
        /// </summary>
        /// <param name="crc">Входная коллекция.</param>
        /// <returns>Рассчитанное значение Сrc.</returns>
        public static byte CrcCalc(this IEnumerable<byte> crc)
        {
            return crc.Aggregate<byte, byte>(0, (current, item) => (byte)(current + item));
        }
    }
}
