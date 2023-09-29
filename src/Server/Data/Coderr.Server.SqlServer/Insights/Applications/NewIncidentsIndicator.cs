using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Applications
{
    [ContainerService]
    public class NewIncidentsIndicator : CounterIndicator
    {
        public NewIncidentsIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "NewIncidentsCount";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var sql = @"select ApplicationId, cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date) CreatedAt, count(*)
                                    from CommonIncidentProgressTracking
                                    where (CreatedAtUtc >= @startDate AND CreatedAtUtc <= @endDate)
                                    #AppIdConstraint#
                                    group by cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date), ApplicationId";
            var items = await ExecuteCountSql(context, sql);

            var builderContext = new CountBuilderContext(context, items);
            var systemInsight = CreateKpiBase("New incidents");
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

            systemInsight = CreateKpiBase("Most new incidents", IndicatorValueComparison.HigherIsBetter, true);
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appBuilderContext = new CountBuilderContext(context, filtered);

                var appInsight = CreateKpiBase("New incidents");
                appBuilderContext.AppCountInsight(appInsight, applicationId, x => x.ApplicationId);
                appInsight.PeriodValueId = null;
                appInsight.ValueId = null;
                appInsight.Toplist = null;
            }

        }

        private static KeyPerformanceIndicator CreateKpiBase(string title, IndicatorValueComparison comparisonType = IndicatorValueComparison.LowerIsBetter, bool isVariation = false, ValueIdType idType = ValueIdType.ApplicationId)
        {
            return new KeyPerformanceIndicator(IndicatorName, title,
                comparisonType)
            {
                IsVariation = isVariation,
                CanBeNormalized = true,
                Comment = "Number of new incidents",
                Description = "Lower numbers indicates better quality as less unique errors have been reported.",
                ValueIdType = idType
            };
        }
    }


    //[ContainerService]
    //public class NewIncidentsMetric : CounterKeyMetricGenerator
    //{
    //    private readonly IAdoNetUnitOfWork _unitOfWork;

    //    public NewIncidentsMetric(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //    }

    //    private const string IndicatorName = "NewIncidentsCount";

    //    public override async Task<KeyMetricDataResult> Collect(KeyMetricGeneratorContext context)
    //    {
    //        IEnumerable<int> appIds;
    //        if (context.ApplicationId == null)
    //        {
    //            appIds = context.User.Claims
    //                .Where(x => x.Type == CoderrClaims.Application)
    //                .Select(x => int.Parse(x.Value));
    //        }
    //        else
    //        {
    //            appIds = new[] { context.ApplicationId.Value };
    //        }
    //        var appIdStr = string.Join(", ", appIds);

    //        var from = DateTime.Today.AddDays(-context.DaysFrom);
    //        var to = DateTime.Today.AddDays(1).AddSeconds(-1).AddDays(-context.DaysTo);


    //        var result = new KeyMetricDataResult { };

    //        if (context.IncludeMetric)
    //        {
    //            var sql2 = $@"select count(IncidentId)
    //                        from CommonIncidentProgressTracking
    //                        where AssignedAtUtc is null
    //                        AND CreatedAtUtc >= @from AND CreatedAtUtc <= @to
    //                        AND ApplicationId IN ({appIdStr})";

    //            result.Application.Metric.Value = (int)_unitOfWork.ExecuteScalar(sql2, new { from, to });
    //        }

    //        else if (context.IncludeTopList && context.ApplicationId == null)
    //        {
    //            var sql2 = $@"select ApplicationId, count(*) Count
    //                        from CommonIncidentProgressTracking
    //                        where AssignedAtUtc is null
    //                        AND CreatedAtUtc >= @from AND CreatedAtUtc <= @to
    //                        AND ApplicationId IN ({appIdStr})
    //                        group by ApplicationId";

    //            var data3 = await _unitOfWork.ToListAsync<ItemWithCount>(sql2, new { startDate = from, endDate = to });

    //            result.Application.Toplist = data3
    //                .Select(x => new MetricToplistItem { Id=x.ApplicationId, Title = context.ApplicationNames[x.ApplicationId], Value = x.Count })
    //                .OrderByDescending(x => x.Value)
    //                .ToArray();

    //            if (result.Application.Toplist.Any())
    //            {
    //                result.Application.Metric.Value = context.ApplicationNames[result.Application.Toplist[0].Id];
    //                result.Application.Metric.Comment = $"{result.Application.Toplist[0].Value} errors";
    //            }
                
    //        }

    //        if (context.IncludeChartData)
    //        {
    //            var sql = $@"select ApplicationId, cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date) When, count(*)
    //                                from CommonIncidentProgressTracking
    //                                where (CreatedAtUtc >= @startDate AND CreatedAtUtc <= @endDate)
    //                                AND ApplicationId IN ({appIdStr})
    //                                group by cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date), ApplicationId";
    //            var data2 = await _unitOfWork.ToListAsync<ItemWithCount>(sql, new { startDate = from, endDate = to });

    //            var perApp = data2.GroupBy(x=>x.)
    //            result.Application.ChartData = data2.Select(x=>new TrendLineValue(x.Count))
    //        }
    //        var data = new KeyMetricDataResult();
    //        data.Application = new KeyMetricDataResultItem
    //        {

    //        }


    //        var items = await ExecuteCountSql(context, sql);

    //        var builderContext = new CountBuilderContext(context, items);
    //        var systemInsight = CreateKpiBase("New incidents");
    //        builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

    //        systemInsight = CreateKpiBase("Most new incidents", IndicatorValueComparison.HigherIsBetter, true);
    //        builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

    //        foreach (var applicationId in context.ApplicationIds)
    //        {
    //            var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
    //            var appBuilderContext = new CountBuilderContext(context, filtered);

    //            var appInsight = CreateKpiBase("New incidents");
    //            appBuilderContext.AppCountInsight(appInsight, applicationId, x => x.ApplicationId);
    //            appInsight.PeriodValueId = null;
    //            appInsight.ValueId = null;
    //            appInsight.Toplist = null;
    //        }

    //    }

    //    private static KeyPerformanceIndicator CreateKpiBase(string title, IndicatorValueComparison comparisonType = IndicatorValueComparison.LowerIsBetter, bool isVariation = false, ValueIdType idType = ValueIdType.ApplicationId)
    //    {
    //        return new KeyPerformanceIndicator(IndicatorName, title,
    //            comparisonType)
    //        {
    //            IsVariation = isVariation,
    //            CanBeNormalized = true,
    //            Comment = "Number of new incidents",
    //            Description = "Lower numbers indicates better quality as less unique errors have been reported.",
    //            ValueIdType = idType
    //        };
    //    }
    //}
}
