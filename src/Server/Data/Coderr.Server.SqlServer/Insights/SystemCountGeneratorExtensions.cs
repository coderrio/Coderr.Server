using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;

namespace Coderr.Server.SqlServer.Insights
{
    public static class SystemCountGeneratorExtensions
    {
        public static void SystemCountInsight(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField = null,
            Func<IGrouping<int, ItemWithCount>, double> topListAggregation = null)
        {
            if (groupField == null)
            {
                groupField = x => x.ApplicationId;
            }
            if (topListAggregation == null)
            {
                topListAggregation = x => x.Sum(y => y.Count);
            }

            context.SystemCountKeyFigure(kpi, groupField);
            context.SystemCountTopList(kpi, groupField, topListAggregation);
            context.SystemCountTrend(kpi, groupField);
            context.IndicatorContext.AddIndicator(0, kpi);
        }

        public static void SystemCountKeyFigure(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField)
        {
            var periodValue = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.PeriodStartDate
                            && x.When <= context.IndicatorContext.PeriodEndDate)
                .GroupBy(groupField)
                .OrderByKpi(y => y.Sum(x => x.Count), kpi)
                .FirstOrDefault();
            kpi.PeriodValue = periodValue?.Sum(x => x.Count).DivideWith(3);
            kpi.PeriodValueId = periodValue?.Key;


            var valueItem = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.ValueStartDate
                            && x.When <= context.IndicatorContext.ValueEndDate)
                .GroupBy(groupField)
                .OrderByKpi(y => y.Sum(x => x.Count), kpi)
                .FirstOrDefault();
            kpi.Value = valueItem?.Sum(x => x.Count);
            kpi.ValueId = valueItem?.Key;
        }

        public static void SystemCountTrend(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField)
        {
            var lines = new List<TrendLine>();
            var incidentsPerAppOrdered = context.Incidents
                .GroupBy(groupField)
                .OrderByKpi(x => x.Sum(y => y.Count), kpi)
                .Take(5);
            foreach (var valuesPerAccount in incidentsPerAppOrdered)
            {
                var values = new TrendLineValue[context.IndicatorContext.TrendDates.Length];
                for (var i = 0; i < context.IndicatorContext.TrendDates.Length; i++)
                {
                    var value = valuesPerAccount
                        .Where(x => x.When == context.IndicatorContext.TrendDates[i])
                        .DefaultIfEmpty()
                        .Sum(x => x?.Count ?? 0);
                    values[i] = new TrendLineValue(value);
                }

                lines.Add(new TrendLine(valuesPerAccount.Key, values));
            }

            kpi.TrendLines = lines.ToArray();
        }

        public static void SystemCountTopList(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField,
            Func<IGrouping<int, ItemWithCount>, double> valueAggregation)
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