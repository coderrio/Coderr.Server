using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Applications
{
    [ContainerService]
    public class WorkTimeIndicator : DurationIndicator
    {

        public WorkTimeIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "WorkTime";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {

            var sql = @"SELECT ApplicationId, 
	                                       cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date) AssignedAt, 
	                                       Duration = AVG(
                                            CASE 
                                                WHEN ClosedAtUtc IS NULL 
                                                THEN DATEDIFF(hour, AssignedAtUtc, GetUtcDate())
		                                        ELSE DATEDIFF(hour, AssignedAtUtc, ClosedAtUtc)
	                                       END / 24.0)
                                    FROM CommonIncidentProgressTracking
                                    WHERE AssignedAtUtc <= @to AND ClosedAtUtc >= @from
                                    #AppIdConstraint#
                                    Group BY ApplicationId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date)";
            var items = await ExecuteCountSql(context, sql);

            var systemBuilderContext = new DurationBuilderContext(context, items);
            var systemInsight = CreateKpiBase("Work duration");
            systemBuilderContext.SystemDurationInsight(systemInsight, x => x.ApplicationId);

            systemInsight = CreateKpiBase("Longest work duration");
            systemInsight.IsVariation = true;
            systemBuilderContext.SystemDurationInsight(systemInsight, x => x.ApplicationId);


            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appContext = new DurationBuilderContext(context, filtered);

                var appInsight = CreateKpiBase("Work duration");
                appContext.AppDurationInsight(appInsight, applicationId, x => x.ApplicationId);
                appInsight.PeriodValueId = null;
                appInsight.ValueId = null;
                appInsight.ValueIdType = ValueIdType.None;
            }
        }

        private KeyPerformanceIndicator CreateKpiBase(string title)
        {
            return new KeyPerformanceIndicator(IndicatorName, title,
                IndicatorValueComparison.LowerIsBetter)
            {
                CanBeNormalized = true,
                Comment = "",
                Description = "Average time it takes to correct an issue.",
                ValueUnit = "day(s)",
                ValueIdType = ValueIdType.ApplicationId
            };
        }

    }
}
