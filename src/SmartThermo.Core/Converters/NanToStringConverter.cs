using System;
using System.Globalization;
using System.Windows.Data;

namespace SmartThermo.Core.Converters
{
    public class NanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && double.IsNaN((double) value) ? "-/-" : $"{value}°C";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
