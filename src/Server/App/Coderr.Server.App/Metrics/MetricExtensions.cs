using System;
using System.Collections.Generic;

namespace Coderr.Server.App.Metrics
{
    public static class MetricExtensions
    {
        private const string WeekSqlFormat = "YEAR(DATEADD(day, 26 - DATEPART(isoww, CreatedAtUtc), CreatedAtUtc))";
        private const string MonthSqlFormat = "DATEADD(MONTH, DATEDIFF(MONTH, 0, CreatedAtUtc), 0)";
        private const string DaySqlFormat = "CONVERT(date, CreatedAtUtc)";


        /// <summary>
        ///     Generate all labels for the chart.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DateTime[] GenerateDates(this IMetricGenerationContext context)
        {
            var date = context.From;
            var dates = new List<DateTime>();
            while (date <= context.To)
            {
                dates.Add(date);
                switch (context.Configuration.Period.TimeType)
                {
                    case TimeType.Days:
                        date = date.AddDays(1);
                        break;
                    case TimeType.Weeks:
                        date = date.AddDays(7);
                        break;
                    case TimeType.Months:
                        date = date.AddMonths(1);
                        break;
                }
            }

            return dates.ToArray();
        }


        public static string GetSqlDateFormat(this TimeType periodTimeType)
        {
            switch (periodTimeType)
            {
                case TimeType.Days:
                    return DaySqlFormat;
                case TimeType.Weeks:
                    return WeekSqlFormat;
                default:
                    return MonthSqlFormat;
            }
        }

    }
}
