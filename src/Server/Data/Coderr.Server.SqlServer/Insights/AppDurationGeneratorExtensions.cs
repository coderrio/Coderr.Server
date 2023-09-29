using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;

namespace Coderr.Server.SqlServer.Insights
{
    public static class AppDurationGeneratorExtensions
    {
        public static void AppDurationInsight(this DurationBuilderContext context,
            KeyPerformanceIndicator kpi,
            int applicationId,
            Func<MonthDuration, int> groupField,
            Func<IGrouping<int, MonthDuration>, double> toplistAggregation = null)
        {
            if (toplistAggregation == null)
            {
                toplistAggregation = x => x.Average(y => y.Duration.TotalDays);
            }

            context.AppDurationKeyFigure(kpi, groupField);
            context.AppDurationTrend(kpi, groupField);
            context.AppDurationTopList(kpi, groupField, toplistAggregation);
            context.IndicatorContext.AddIndicator(applicationId, kpi);
        }

        public static void AppDurationKeyFigure(this DurationBuilderContext context,
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

        public static void AppDurationTrend(this DurationBuilderContext context,
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
                double lastValue = 0;
                var values = new TrendLineValue[context.IndicatorContext.TrendDates.Length];
                for (var i = 0; i < context.IndicatorContext.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .Where(x => x.When == context.IndicatorContext.TrendDates[i])
                        .DefaultIfEmpty()
                        .Average(x => x?.Duration.TotalDays ?? 0);
                    if (value < 0.5)
                    {
                        values[i] = new TrendLineValue(lastValue);
                    }
                    else
                    {
                        values[i] = new TrendLineValue(value);
                        lastValue = value;
                    }
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
        }

        public static void AppDurationTopList(this DurationBuilderContext context,
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