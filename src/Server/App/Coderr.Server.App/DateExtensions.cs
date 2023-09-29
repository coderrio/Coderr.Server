using System;

namespace Coderr.Server.App
{
    public static class DateExtensions
    {
        public static DateTime ToFirstDayOfMonth(this DateTime time)
        {
            return new DateTime(time.Year, time.Month, 1);
        }
    }
}
