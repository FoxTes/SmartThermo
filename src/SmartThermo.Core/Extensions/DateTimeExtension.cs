using System;

namespace SmartThermo.Core.Extensions
{
    public static class DateTimeExtension
    {
        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval,
            MidpointRounding roundingType = MidpointRounding.ToEven)
        {
            return new TimeSpan(
                Convert.ToInt64(Math.Round(time.Ticks / (decimal) roundingInterval.Ticks, roundingType)) *
                roundingInterval.Ticks);
        }

        public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval)
        {
            return new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);
        }
    }
}
