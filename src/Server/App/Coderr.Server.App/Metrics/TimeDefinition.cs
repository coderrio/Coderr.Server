using System;

namespace Coderr.Server.App.Metrics
{
    public enum TimeType
    {
        Days,
        Weeks,
        Months
    }

    /// <summary>
    ///     Allows us to define an running period.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For instance, if we say 6 months we want to
    ///     </para>
    ///     <para>
    ///         Do note that only one of the fields must be specified.
    ///     </para>
    /// </remarks>
    public class TimeDefinition
    {
        public TimeDefinition(TimeType timeType, int @from, int to)
        {
            if (from < to)
            {
                throw new ArgumentOutOfRangeException(nameof(from), from,
                    "Must be higher than to (since it should go back longer in the time from today).");
            }
            TimeType = timeType;
            From = @from;
            To = to;
        }

        /// <summary>
        /// Adjust the to period so it's a monday or first day of month.
        /// </summary>
        public bool ToBeginningOfPeriod { get; set; }

        public TimeType TimeType { get; private set; }

        /// <summary>
        /// Amount of days back from today (start of range)
        /// </summary>
        public int From { get; private set; }
        /// <summary>
        /// Amount of days back from today (end of range
        /// </summary>
        public int To { get; private set; }

        public Tuple<DateTime, DateTime> ToDate()
        {
            var fromDate = DateTime.Today;

            switch (TimeType)
            {
                case TimeType.Days:
                    {
                        fromDate = fromDate.AddDays(-From);
                        if (ToBeginningOfPeriod)
                        {
                            fromDate = fromDate.DayOfWeek switch
                            {
                                DayOfWeek.Saturday => fromDate.AddDays(-1),
                                DayOfWeek.Sunday => fromDate.AddDays(1),
                                _ => fromDate
                            };
                        }

                        break;
                    }
                case TimeType.Weeks:
                    {
                        fromDate = fromDate.AddDays(-(From * 7));
                        if (ToBeginningOfPeriod)
                        {
                            while (fromDate.DayOfWeek != DayOfWeek.Monday)
                            {
                                fromDate = fromDate.AddDays(-1);
                            }
                        }

                        break;
                    }
                case TimeType.Months:
                    {
                        fromDate = fromDate.AddMonths(-From);
                        if (ToBeginningOfPeriod)
                        {
                            fromDate = fromDate.ToFirstDayOfMonth();
                        }
                    }
                    break;

                default:
                    throw new InvalidOperationException("Nothing was configured");
            }

            var toDate = fromDate.AddDays(To - From);
            return new Tuple<DateTime, DateTime>(fromDate, toDate);

        }
    }
}