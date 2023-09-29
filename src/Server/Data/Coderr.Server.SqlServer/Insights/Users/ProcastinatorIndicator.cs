using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Users
{

    [ContainerService]
    public class ProcastinatorIndicator : DurationIndicator
    {
        public ProcastinatorIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "Procastinator";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {

            var sql = @"SELECT ApplicationId, 
	                                       cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date) AssignedAt, 
	                                       Duration = AVG(
                                            CASE 
                                                WHEN ClosedAtUtc IS NULL 
                                                THEN DATEDIFF(hour, AssignedAtUtc, GetUtcDate())
		                                        ELSE DATEDIFF(hour, AssignedAtUtc, ClosedAtUtc)
	                                       END / 24.0), AssignedToId
                                    FROM CommonIncidentProgressTracking
                                    WHERE AssignedAtUtc <= @to AND ClosedAtUtc >= @from
                                    #AppIdConstraint#
                                    Group BY ApplicationId, AssignedToId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date)";
            var items = await ExecuteCountSql(context, sql, true);

            var kpi = CreateKpiBase("The procrastinator");
            var durationContext = new DurationBuilderContext(context, items);
            durationContext.SystemDurationInsight(kpi, x => x.AccountId, x => x.Average(y => y.Duration.TotalDays));

            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                kpi = CreateKpiBase("The procrastinator");
                var appDurationContext = new DurationBuilderContext(context, filtered);
                appDurationContext.AppDurationInsight(kpi, applicationId, x => x.AccountId);
            }
        }

        private KeyPerformanceIndicator CreateKpiBase(string title)
        {
            return new KeyPerformanceIndicator(IndicatorName, title,
                IndicatorValueComparison.HigherIsBetter)
            {
                CanBeNormalized = true,
                Comment = "",
                Description = "Person that starts working on many incidents but never finish them.",
                ValueIdType = ValueIdType.AccountId,
                ValueUnit = "day(s)"
            };
        }

    }
}
