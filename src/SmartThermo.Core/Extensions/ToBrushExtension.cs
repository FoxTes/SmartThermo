using System.Windows.Media;
using Color = System.Drawing.Color;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Предоставляет методы, позволяющие конвертировать цвет.
    /// </summary>
    public static class ToBrushExtension
    {
        /// <summary>
        /// Конвертирует цвет из Color в SolidColorBrush.
        /// </summary>
        /// <param name="color">Входной цвет.</param>
        /// <returns>SolidColorBrush.</returns>
        public static Brush ToBrush(this Color color)
        {
            return new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }
    }
}
