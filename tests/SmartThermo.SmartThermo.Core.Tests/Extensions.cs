using System.Collections.Generic;
using System.Linq;
using SmartThermo.Core.Extensions;
using Xunit;

namespace SmartThermo.SmartThermo.Core.Tests
{
    public class Extensions
    {
        [Fact]
        public void CrcCalcTest()
        {
            var data = new byte[] { 0x01, 0x01 };
            var result = data.CrcCalc();
            Assert.Equal(0x02, result);

            var data1 = new byte[] { 0x00, 0x00 };
            var result1 = data1.CrcCalc();
            Assert.Equal(0x00, result1);

            var data2 = new byte[] { 0x01, 0xFF };
            var result2 = data2.CrcCalc();
            Assert.Equal(0x00, result2);

            var data3 = new byte[] { 0x02, 0xFF };
            var result3 = data3.CrcCalc();
            Assert.Equal(0x01, result3);
        }
        
        [Fact]
        public void SplitByTest()
        {
            var values = new List<double>
            {
                1.2, 2.2, 3.2,
                double.NaN,
                double.NaN,
                double.NaN,
                2.2, 2.3, 
                double.NaN,
                4.1, 4.2, 4.3
            };
            var valuesList = new List<List<double>>()
            {
                new List<double> { 1.2, 2.2, 3.2},
                new List<double> { 2.2, 2.3},
                new List<double> { 4.1, 4.2, 4.3},
            };
            
            var result = values.SplitBy(double.NaN)
                .ToList();
            Assert.Equal(result, valuesList);
            
            var result1 = values.SplitBy(double.PositiveInfinity)
                .ToList();
            Assert.NotEqual(result1, valuesList);
        }
    }
}
