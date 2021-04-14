using SmartThermo.Core.Extensions;
using Xunit;

namespace SmartThermo.SmartThermo.Core.Tests
{
    public class Extensions
    {
        [Fact]
        public void CrcCalcTest()
        {
            byte[] data = new byte[2] { 0x01, 0x01 };
            var result = data.CrcCalc();
            Assert.Equal(0x02, result);

            byte[] data1 = new byte[2] { 0x00, 0x00 };
            var result1 = data1.CrcCalc();
            Assert.Equal(0x00, result1);

            byte[] data2 = new byte[2] { 0x01, 0xFF };
            var result2 = data2.CrcCalc();
            Assert.Equal(0x00, result2);

            byte[] data3 = new byte[2] { 0x02, 0xFF };
            var result3 = data3.CrcCalc();
            Assert.Equal(0x01, result3);
        }
    }
}
