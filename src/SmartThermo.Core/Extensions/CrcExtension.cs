using System.Collections.Generic;
using System.Linq;

namespace SmartThermo.Core.Extensions
{
    public static class CrcExtension
    {
        public static byte CrcCalc(this IEnumerable<byte> crc)
        {
            return crc.Aggregate<byte, byte>(0, (current, item) => (byte)(current + item));
        }
    }
}
