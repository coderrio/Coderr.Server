using System;

namespace codeRR.Server.App.Modules.Messaging.Templating
{
    /// <summary>
    ///     Used to format dates into different representations like duration from now.
    /// </summary>
    public static class DateFormatter
    {
        /// <summary>
        ///     Display elapsed time from the given date
        /// </summary>
        /// <param name="specifiedTime">Time to diff from (local time)</param>
        /// <returns>English string</returns>
        public static string ElapsedTime(DateTime specifiedTime)
        {
            var difference = DateTime.Now.Subtract(specifiedTime);

            var years = (int) (difference.TotalDays/365);
            if (years >= 1)
                return string.Format("{0} {1} ago", years, years == 1 ? "year" : "years");

            var months = (int) (difference.TotalDays/30);
            if (months >= 1)
                return string.Format("{0} {1} ago", months, months == 1 ? "month" : "months");

            var weeks = (int) (difference.TotalDays/7);
            if (weeks >= 1)
                return string.Format("{0} {1} ago", weeks, weeks == 1 ? "week" : "weeks");

            var days = (int) difference.TotalDays;
            if (days >= 1)
                return string.Format("{0} {1} ago", days, days == 1 ? "day" : "days");

            var hours = (int) difference.TotalHours;
            if (hours >= 1)
                return string.Format("{0} {1} ago", hours, hours == 1 ? "hour" : "hours");

            var minutes = (int) difference.TotalMinutes;
            if (minutes >= 1)
                return string.Format("{0} {1} ago", minutes, minutes == 1 ? "minute" : "minutes");

            return "moments ago";
        }

        /// <summary>
        ///     Generates a text representing the period of time from now to the given future date
        /// </summary>
        /// <param name="specifiedTime">Date in the future (local time)</param>
        /// <returns>For instance "in two days"</returns>
        public static string FutureTime(DateTime specifiedTime)
        {
            var difference = DateTime.Now.Subtract(specifiedTime);

            var years = (int) (difference.TotalDays/365);
            if (years >= 1)
                return string.Format("in {0} {1}", years, years == 1 ? "year" : "years");

            var months = (int) (difference.TotalDays/30);
            if (months >= 1)
                return string.Format("in {0} {1}", months, months == 1 ? "month" : "months");

            var weeks = (int) (difference.TotalDays/7);
            if (weeks >= 1)
                return string.Format("in {0} {1}", weeks, weeks == 1 ? "week" : "weeks");

            var days = (int) difference.TotalDays;
            if (days >= 1)
                return string.Format("in {0} {1}", days, days == 1 ? "day" : "days");

            var hours = (int) difference.TotalHours;
            if (hours >= 1)
                return string.Format("in {0} {1}", hours, hours == 1 ? "hour" : "hours");

            var minutes = (int) difference.TotalMinutes;
            if (minutes >= 1)
                return string.Format("in {0} {1}", minutes, minutes == 1 ? "minute" : "minutes");

            return "in a moment";
        }
    }
}