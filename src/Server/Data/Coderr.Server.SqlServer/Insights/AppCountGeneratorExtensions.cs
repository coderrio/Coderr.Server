using System;
using System.Collections.Generic;
using System.Linq;
using Coderr.Client;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.App.Insights.Keyfigures.Application;

namespace Coderr.Server.SqlServer.Insights
{
    public static class AppCountGeneratorExtensions
    {
        public static void AppCountInsight(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            int applicationId,
            Func<ItemWithCount, int> groupField,
            Func<IGrouping<int, ItemWithCount>, double> valueAggregation = null)
        {
            if (valueAggregation == null)
            {
                valueAggregation = x => x.Sum(y => y.Count);
            }

            context.AppCountKpi(kpi, groupField);
            context.AppAccountCountTopList(kpi, groupField, valueAggregation);
            context.AppCountTrend(kpi, groupField);
            context.IndicatorContext.AddIndicator(applicationId, kpi);
        }

        /// <summary>
        /// We group
        /// </summary>
        /// <param name="context"></param>
        /// <param name="kpi"></param>
        /// <param name="groupField"></param>

        public static void AppCountKpi(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField)
        {
            var periodValue = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.PeriodStartDate
                            && x.When <= context.IndicatorContext.PeriodEndDate)
                .GroupBy(groupField)
                .FirstOrDefault();
            kpi.PeriodValue = periodValue?.Sum(x => x.Count).DivideWith(3);
            kpi.PeriodValueId = periodValue?.Key;


            var valueItem = context.Incidents
                .Where(x => x.When >= context.IndicatorContext.ValueStartDate
                            && x.When <= context.IndicatorContext.ValueEndDate)
                .GroupBy(groupField)
                .FirstOrDefault();
            kpi.Value = valueItem?.Sum(x => x.Count);
            kpi.ValueId = valueItem?.Key;
        }

        public static void AppCountTrend(this CountBuilderContext context,
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
                if (valuesPerAccount.Key == 0)
                {
                    Err.ReportLogicError("Expected an accountId, got 0.", new
                    {
                        apps = incidentsPerAppOrdered.Select(x => x.Key).ToArray(),
                        kpi
                    });
                    continue;
                }

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

        public static void AppCountTrend2(this CountBuilderContext context,
            KeyPerformanceIndicator kpi,
            Func<ItemWithCount, int> groupField)
        {
            var lines = new List<TrendLine>();
            var values = new TrendLineValue[context.IndicatorContext.TrendDates.Length];
            for (var i = 0; i < context.IndicatorContext.TrendDates.Length; i++)
            {
                var value = context.Incidents
                    .Where(x => x.When == context.IndicatorContext.TrendDates[i])
                    .Sum(x => x.Count);
                values[i] = new TrendLineValue(value);
            }

            lines.Add(new TrendLine("Count", values));
            kpi.TrendLines = lines.ToArray();
        }

        /// <summary>
        /// For the toplist, we take the average values for each application/account
        /// </summary>
        /// <param name="context"></param>
        /// <param name="kpi"></param>
        /// <param name="groupField"></param>
        /// <param name="valueAggregation"></param>
        public static void AppAccountCountTopList(this CountBuilderContext context,
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