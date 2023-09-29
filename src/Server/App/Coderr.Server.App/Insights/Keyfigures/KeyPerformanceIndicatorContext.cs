using System;
using System.Collections.Generic;

namespace Coderr.Server.App.Insights.Keyfigures
{
    public class KeyPerformanceIndicatorContext
    {
        private readonly Dictionary<int, List<KeyPerformanceIndicator>> _indicators;

        public KeyPerformanceIndicatorContext(Dictionary<int, List<KeyPerformanceIndicator>> indicators)
        {
            _indicators = indicators;
            ValueStartDate = DateTime.Today.AddDays(-30);
            ValueEndDate = DateTime.UtcNow;
            PeriodStartDate = DateTime.Today.AddDays(-90);
            PeriodEndDate = DateTime.UtcNow;
            StartDate = DateTime.Today.AddDays(-365).ToFirstDayOfMonth();
            EndDate = DateTime.UtcNow;

            var dates = new List<DateTime>();
            var date = StartDate;
            while (date < DateTime.Today)
            {
                dates.Add(date);
                date = date.AddMonths(1);
            }

            TrendDates = dates.ToArray();
        }

        /// <summary>
        ///     Several ids = calculate averages
        /// </summary>
        public int[] ApplicationIds { get; set; }

        /// <summary>
        ///     Fetch data to this date (inclusive).
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        ///     Period summary, shown at the bottom of the KPI. Value is typically today - 90 days (the last quarter)
        /// </summary>
        public DateTime PeriodEndDate { get; set; }

        /// <summary>
        ///     End date for the period (shown at the bottom of the KPI), typically "now".
        /// </summary>
        public DateTime PeriodStartDate { get; set; }

        /// <summary>
        ///     Fetch data from this date (includes both trend dates and value dates)
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        ///     Dates for trends (collect values between the given dates)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Can be monthly for the last year, quarterly or half years
        ///     </para>
        /// </remarks>
        public DateTime[] TrendDates { get; set; }

        /// <summary>
        ///     End date for the value to show in the KPI (typically "now")
        /// </summary>
        public DateTime ValueEndDate { get; set; }

        /// <summary>
        ///     Start date for the value to show in the KPI (typically start of this month)
        /// </summary>
        public DateTime ValueStartDate { get; set; }


        public void AddIndicator(int applicationId, KeyPerformanceIndicator indicator)
        {
            if (!_indicators.TryGetValue(applicationId, out var indicators))
            {
                indicators = new List<KeyPerformanceIndicator>();
                _indicators[applicationId] = indicators;
            }

            indicators.Add(indicator);
        }
    }
}