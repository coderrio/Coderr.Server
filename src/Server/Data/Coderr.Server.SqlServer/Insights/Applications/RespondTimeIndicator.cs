using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Applications
{
    [ContainerService]
    class RespondTimeIndicator : DurationIndicator
    {
        public RespondTimeIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string InidcatorName = "RespondTime";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var sql = @"SELECT ApplicationId, 
	                                       cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date) CreatedAt, 
	                                       Duration = AVG(
                                            CASE 
                                                WHEN AssignedAtUtc IS NULL 
                                                THEN DATEDIFF(hour, CreatedAtUtc, GetUtcDate())
		                                        ELSE DATEDIFF(hour, CreatedAtUtc, AssignedAtUtc)
	                                       END / 24.0), AssignedToId
                                    FROM CommonIncidentProgressTracking
                                    WHERE CreatedAtUtc <= @to AND AssignedAtUtc >= @from
                                    #AppIdConstraint#
                                    Group BY ApplicationId, AssignedToId, cast(FORMAT(CreatedAtUtc,'yyyy-MM-01') as date)";


            var items = await base.ExecuteCountSql(context, sql, true);

            var systemBuilderContext = new DurationBuilderContext(context, items);
            var systemInsight = CreateKpiBase("Respond time");
            systemBuilderContext.SystemDurationInsight(systemInsight, x => x.ApplicationId);

            systemInsight = CreateKpiBase("Highest response time");
            systemInsight.IsVariation = true;
            systemBuilderContext.SystemDurationInsight(systemInsight, x => x.ApplicationId);

            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appContext = new DurationBuilderContext(context, filtered);

                var appInsight = CreateKpiBase("Respond time");
                appContext.AppDurationInsight(appInsight, applicationId, x => x.ApplicationId);
                appInsight.PeriodValueId = null;
                appInsight.ValueId = null;

                var appInsight2 = CreateKpiBase("Respond time (per developer)", IndicatorValueComparison.HigherIsBetter, true, ValueIdType.AccountId);
                appInsight2.ValueIdType = ValueIdType.AccountId;
                appContext.AppDurationInsight(appInsight2, applicationId, x => x.AccountId);

                appInsight.Toplist = appInsight2.Toplist;
                appInsight.ToplistValueType = appInsight2.ToplistValueType;
            }
        }


        private KeyPerformanceIndicator CreateKpiBase(string title, IndicatorValueComparison comparisonType = IndicatorValueComparison.LowerIsBetter, bool isVariation = false, ValueIdType idType = ValueIdType.ApplicationId)
        {
            return new KeyPerformanceIndicator(InidcatorName, title,
                comparisonType)
            {
                CanBeNormalized = true,
                Comment = "",
                Description =
                    "Number of days (in average) that it takes from the first error report until someone starts to correct an error",
                IsVariation = isVariation,
                ValueIdType = idType,
                ValueUnit = "day(s)"
            };
        }
    }
}
