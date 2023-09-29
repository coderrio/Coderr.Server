using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;

namespace Coderr.Server.SqlServer.Insights
{
    public static class SystemDurationGeneratorExtensions
    {
        public static void SystemDurationInsight(this DurationBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<MonthDuration, int> groupField, 
            Func<IGrouping<int, MonthDuration>, double> toplistAggregation = null)
        {
            if (toplistAggregation == null)
            {
                toplistAggregation = z => z.Average(x => x.Duration.TotalDays);
            }
            context.SystemDurationKpi(kpi, groupField);
            context.SystemDurationTrend(kpi, groupField);
            context.SystemDurationTopList(kpi, groupField, toplistAggregation);
            context.IndicatorContext.AddIndicator(0, kpi);
        }

        public static void SystemDurationKpi(this DurationBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<MonthDuration, int> groupField)
        {
            var periodValue = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.PeriodStartDate
                            && x.When <= context.IndicatorContext.PeriodEndDate)
                .GroupBy(groupField)
                .OrderByKpi(y => y.Average(x => x.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodValue?.Average(x => x.Duration.TotalDays);
            kpi.PeriodValueId = periodValue?.Key;


            var valueItem = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.ValueStartDate
                            && x.When <= context.IndicatorContext.ValueEndDate)
                .GroupBy(groupField)
                .OrderByKpi(y => y.Average(x => x.Duration.TotalDays), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Average(x => x.Duration.TotalDays);
            kpi.ValueId = valueItem?.Key;
        }

        public static void SystemDurationTrend(this DurationBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<MonthDuration, int> groupField)
        {
            var lines = new List<TrendLine>();
            var incidentsPerAppOrdered = context.Incidents
                .GroupBy(groupField)
                .OrderByKpi(x => x.Count(), kpi)
                .Take(5);
            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.IndicatorContext.TrendDates.Length];
                for (var i = 0; i < context.IndicatorContext.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .Where(x => x.When == context.IndicatorContext.TrendDates[i])
                        .DefaultIfEmpty()
                        .Average(x => x?.Duration.TotalDays ?? 0);
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
        }

        public static void SystemDurationTopList(this DurationBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<MonthDuration, int> groupField,
            Func<IGrouping<int, MonthDuration>, double> valueAggregation)
        {
            kpi.Toplist = context.Incidents
                .GroupBy(groupField)
                .Where(x => x.Any())
                .Select(x => new KeyPerformanceIndicatorTopListItem
                {
                    ValueId = x.Key,
                    Value = valueAggregation(x),
                    ValueName = kpi.ValueUnit
                })
                .OrderByKpi(x => x.Value, kpi)
                .Take(10)
                .ToArray();
        }
    }
}