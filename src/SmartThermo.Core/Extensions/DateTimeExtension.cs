using System;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Расширения для работы с датой.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Округляет время до указанного интервала.
        /// </summary>
        /// <param name="datetime">Исходное время.</param>
        /// <param name="roundingInterval">Интервал округления.</param>
        /// <returns></returns>
        public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval)
        {
            return new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);
        }

        private static TimeSpan Round(
            this TimeSpan time,
            TimeSpan roundingInterval,
            MidpointRounding roundingType = MidpointRounding.ToEven)
        {
            return new TimeSpan(
                Convert.ToInt64(Math.Round(time.Ticks / (decimal)roundingInterval.Ticks, roundingType)) *
                roundingInterval.Ticks);
        }
    }
}
