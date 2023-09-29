using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;

namespace Coderr.Server.SqlServer.Insights
{
    public class CountBuilderContext
    {
        public CountBuilderContext(KeyPerformanceIndicatorContext indicatorContext, IReadOnlyList<ItemWithCount> incidents)
        {
            IndicatorContext = indicatorContext;
            Incidents = incidents;
        }

        public KeyPerformanceIndicatorContext IndicatorContext { get; private set; }
        public IReadOnlyList<ItemWithCount> Incidents { get; private set; }
    }

    public class DurationBuilderContext
    {
        public DurationBuilderContext(KeyPerformanceIndicatorContext indicatorContext, IReadOnlyList<MonthDuration> incidents)
        {
            IndicatorContext = indicatorContext;
            Incidents = incidents;
        }

        public KeyPerformanceIndicatorContext IndicatorContext { get; private set; }
        public IReadOnlyList<MonthDuration> Incidents { get; private set; }
    }


    public static class GeneratorExtensions
    {
        public static int? AsInt(this double? value)
        {
            if (value == null)
                return null;

            return Convert.ToInt32(value);
        }

        public static int AsInt(this double value)
        {
            return Convert.ToInt32(value);
        }


        /// <summary>
        /// Generate an incident count indicator for a specific application.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="kpi"></param>
        /// <param name="applicationId"></param>
        /// <param name="incidents"></param>
        /// <remarks>
        ///<para>
        ///Toplist are for developers.
        /// Trend is the average count trend (no grouping)
        /// Value is total
        /// </para>
        /// </remarks>
        public static void BuildIncidentCountForApplication(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, int applicationId, IReadOnlyCollection<ItemWithCount> incidents)
        {
            var periodValue = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .DefaultIfEmpty()
                .Sum(y => y?.Count ?? 0);
            kpi.PeriodValue = periodValue;


            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .DefaultIfEmpty()
                .Sum(y => y?.Count ?? 0);
            kpi.Value = valueItem;

            kpi.Toplist = incidents
                .GroupBy(x => x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Average(y => y.Count).AsInt(),
                    ValueName = kpi.ValueUnit
                })
                .OrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();
            var values = new TrendLineValue[context.TrendDates.Length];
            for (var i = 0; i < context.TrendDates.Length; i++)
            {
                var value = incidents
                    .Where(x => x.When == context.TrendDates[i])
                    .Sum(x => x.Count);
                values[i] = new TrendLineValue(value);
            }

            lines.Add(new TrendLine("Count", values));

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(applicationId, kpi);
        }

        /// <summary>
        /// AssignCount
        /// </summary>
        /// <param name="context"></param>
        /// <param name="kpi"></param>
        /// <param name="applicationId"></param>
        /// <param name="incidents"></param>
        public static void BuildIncidentCountPerAccountForApplication(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, int applicationId, IReadOnlyCollection<ItemWithCount> incidents)
        {
            var periodValue = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .Sum(x => x.Count);
            kpi.PeriodValue = periodValue;


            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .Sum(x => x.Count);
            kpi.Value = valueItem;


            kpi.Toplist = incidents
                .GroupBy(x => x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Sum(y => y.Count),
                    ValueName = kpi.ValueUnit
                })
                .OrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();
            var incidentsPerAppOrdered = incidents
                .GroupBy(x => x.AccountId)
                .OrderByKpi(x => x.Count(), kpi)
                .Take(5);
            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Count ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(applicationId, kpi);
        }

        public static void BuildAccountWorstCountIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, int applicationId, IReadOnlyCollection<ItemWithCount> incidents)
        {
            var periodValue = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .GroupBy(x => x.AccountId)
                .ReverseOrderByKpi(x => x.Count(), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodValue?.Sum(x => x.Count);
            kpi.PeriodValueId = periodValue?.Key;


            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .GroupBy(x => x.AccountId)
                .ReverseOrderByKpi(x => x.Count(), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Sum(x => x.Count); ;
            kpi.PeriodValueId = valueItem?.Key;

            var lines = new List<TrendLine>();
            var incidentsPerAppOrdered = incidents
                .GroupBy(x => x.AccountId)
                .ReverseOrderByKpi(x => x.Count(), kpi)
                .Take(5);
            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Count ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(applicationId, kpi);
        }


        public static void BuildAppDurationIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, int applicationId, IReadOnlyCollection<MonthDuration> incidents)
        {
            // company with longest work duration during 90 days
            var periodValue = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .DefaultIfEmpty()
                .Average(y => y?.Duration.TotalDays ?? 0);
            kpi.PeriodValue = periodValue;

            // company with longest work duration during 30 days
            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .DefaultIfEmpty()
                .Average(y => y?.Duration.TotalDays ?? 0);
            kpi.Value = valueItem;

            var lines = new List<TrendLine>();
            var values = new TrendLineValue[context.TrendDates.Length];
            for (var i = 0; i < context.TrendDates.Length; i++)
            {
                var value = incidents
                    .Where(x => x.When == context.TrendDates[i])
                    .DefaultIfEmpty()
                    .Average(x => x?.Duration.TotalDays ?? 0);
                values[i] = new TrendLineValue(value);
            }

            lines.Add(new TrendLine("Average duration (days)", values));

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(applicationId, kpi);
        }


        public static void BuildSystemCountIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, IReadOnlyCollection<ItemWithCount> incidents)
        {
            // Number of incidents during the period grouped by app/account
            var periodItem = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .GroupBy(x => x.ApplicationId)
                .OrderByDescending(x => x.Average(y => y.Count))
                .FirstOrDefault();
            kpi.PeriodValue = periodItem?.Average(x => x.Count).AsInt() ?? 0;
            kpi.PeriodValueId = periodItem?.Key ?? 0;

            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .GroupBy(x => x.ApplicationId)
                .OrderByKpi(x => x.Average(y => y.Count), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Average(x => x.Count).AsInt() ?? 0;
            kpi.ValueId = valueItem?.Key ?? 0;


            kpi.Toplist = incidents
                .GroupBy(x => kpi.ToplistValueType == ValueIdType.ApplicationId ? x.ApplicationId : x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Average(y => y.Count).AsInt(),
                    ValueName = kpi.ValueUnit
                })
                .OrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();

            var line = CreateAverageTrendLine(context, incidents);
            lines.Add(line);

            var incidentsPerAppOrdered = incidents
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Count(), kpi)
                .Take(5);
            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Count ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(0, kpi);
        }

        public static void BuildSystemDurationIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, IReadOnlyCollection<MonthDuration> incidents)
        {
            // company with longest work duration during 90 days
            var periodItem = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .GroupBy(x => x.ApplicationId)
                .OrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodItem?.Average(x => x.Duration.TotalDays).AsInt() ?? 0;
            kpi.PeriodValueId = periodItem?.Key;

            // company with longest work duration during 30 days
            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .GroupBy(x => x.ApplicationId)
                .OrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Average(x => x.Duration.TotalDays).AsInt() ?? 0;
            kpi.ValueId = valueItem?.Key;


            kpi.Toplist = incidents
                .GroupBy(x => kpi.ToplistValueType == ValueIdType.ApplicationId ? x.ApplicationId : x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Average(y => y.Duration.TotalDays),
                    ValueName = "incidents"
                })
                .OrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();

            var line = CreateAverageTrendLine(context, incidents);
            lines.Add(line);

            // Best apps
            var appIncidentsSorted = incidents
                .GroupBy(x => x.ApplicationId)
                .OrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .Take(5);
            foreach (var valuesPerApp in appIncidentsSorted)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerApp
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Duration.TotalDays ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerApp.Key, values));
            }



            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(0, kpi);
        }

        public static void BuildSystemWorstDurationIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, IReadOnlyCollection<MonthDuration> incidents)
        {
            kpi.IsVariation = true;

            // company with longest work duration during 90 days
            var periodItem = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodItem?.Average(x => x.Duration.TotalDays).AsInt() ?? 0;
            kpi.PeriodValueId = periodItem?.Key;

            // company with longest work duration during 30 days
            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Average(x => x.Duration.TotalDays).AsInt() ?? 0;
            kpi.ValueId = valueItem?.Key;


            kpi.Toplist = incidents
                .GroupBy(x => kpi.ToplistValueType == ValueIdType.ApplicationId ? x.ApplicationId : x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Average(y => y.Duration.TotalDays),
                    ValueName = "incidents"
                })
                .ReverseOrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();
            var line2 = CreateAverageTrendLine(context, incidents);
            lines.Add(line2);

            var appIncidentsSorted = incidents
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Average(y => y.Duration.TotalDays), kpi)
                .Take(5);
            foreach (var valuesPerApp in appIncidentsSorted)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerApp
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Duration.TotalDays ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerApp.Key, values));
            }


            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(0, kpi);
        }

        public static void BuildWorstSystemCountIndicator(this KeyPerformanceIndicatorContext context,
            KeyPerformanceIndicator kpi, IReadOnlyCollection<ItemWithCount> incidents)
        {
            kpi.IsVariation = true;

            // Number of incidents during the period grouped by app/account
            var periodItem = incidents
                .Where(x => x.When >= context.PeriodStartDate
                            && x.When <= context.PeriodEndDate)
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Average(y => y.Count), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodItem?.Average(x => x.Count).AsInt() ?? 0;
            kpi.PeriodValueId = periodItem?.Key ?? 0;

            var valueItem = incidents
                .Where(x => x.When >= context.ValueStartDate
                            && x.When <= context.ValueEndDate)
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Average(y => y.Count), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Average(x => x.Count).AsInt() ?? 0;
            kpi.ValueId = valueItem?.Key ?? 0;


            kpi.Toplist = incidents
                .GroupBy(x => kpi.ToplistValueType == ValueIdType.ApplicationId ? x.ApplicationId : x.AccountId)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = x.Average(y => y.Count).AsInt(),
                    ValueName = "incidents"
                })
                .ReverseOrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();

            var lines = new List<TrendLine>();
            var line = CreateAverageTrendLine(context, incidents);
            lines.Add(line);

            var incidentsPerAppOrdered = incidents
                .GroupBy(x => x.ApplicationId)
                .ReverseOrderByKpi(x => x.Count(), kpi)
                .Take(5);

            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.TrendDates.Length];
                for (var i = 0; i < context.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .FirstOrDefault(x => x.When == context.TrendDates[i])?.Count ?? 0;
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
            context.AddIndicator(0, kpi);
        }

        public static IOrderedEnumerable<TSource> OrderByKpi<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, KeyPerformanceIndicator indicator)
        {
            return indicator.ComparisonType == IndicatorValueComparison.LowerIsBetter
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }

        public static IOrderedEnumerable<TSource> ReverseOrderByKpi<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector, KeyPerformanceIndicator indicator)
        {
            return indicator.ComparisonType != IndicatorValueComparison.LowerIsBetter
                ? source.OrderBy(keySelector)
                : source.OrderByDescending(keySelector);
        }

        private static TrendLine CreateAverageTrendLine(KeyPerformanceIndicatorContext context,
            IReadOnlyCollection<MonthDuration> incidents)
        {
            var values2 = new TrendLineValue[context.TrendDates.Length];
            for (var i = 0; i < context.TrendDates.Length; i++)
            {
                var value = incidents
                    .Where(x => x.When == context.TrendDates[i])
                    .DefaultIfEmpty()
                    .Average(x => x?.Duration.TotalDays ?? 0);
                values2[i] = new TrendLineValue(value);
            }

            var line = new TrendLine("Company average", values2)
            {
                IsCompanyAverage = true
            };
            return line;
        }

        private static TrendLine CreateAverageTrendLine(KeyPerformanceIndicatorContext context,
            IReadOnlyCollection<ItemWithCount> incidents)
        {
            var values2 = new TrendLineValue[context.TrendDates.Length];
            for (var i = 0; i < context.TrendDates.Length; i++)
            {
                var value = incidents
                    .Where(x => x.When == context.TrendDates[i])
                    .DefaultIfEmpty()
                    .Average(x => x.Count);
                values2[i] = new TrendLineValue(value);
            }

            var line = new TrendLine("Company average", values2)
            {
                IsCompanyAverage = true
            };
            return line;
        }
    }
}