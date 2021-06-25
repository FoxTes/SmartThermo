using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartThermo.Core.Converters
{
    /// <summary>
    /// Конвертор для преобзования null в string.
    /// </summary>
    /// <remarks>
    /// Используется для отображения температуры.
    /// </remarks>
    [ValueConversion(null, typeof(string))]
    public class NanToStringConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && double.IsNaN((double)value) ? "-/-" : $"{value}°C";
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
