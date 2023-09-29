using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Users
{
    [ContainerService]
    class TheOptimistIndicator : CounterIndicator
    {

        public TheOptimistIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "TheOptimist";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var sql = @"select ApplicationId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date), count(*), AssignedToId
                                    from CommonIncidentProgressTracking
                                    where (AssignedAtUtc >= @startDate AND AssignedAtUtc <= @endDate)
                                    #AppIdConstraint#
                                    group by ApplicationId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date), AssignedToId";
            var items = await ExecuteCountSql(context, sql, true);


            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appContext = new CountBuilderContext(context, filtered);
                var kpi = CreateKpiBase("The Optimist");
                appContext.AppCountInsight(kpi, applicationId, x => x.AccountId);
            }
        }

        private KeyPerformanceIndicator CreateKpiBase(string title)
        {
            return new KeyPerformanceIndicator(IndicatorName, title,
                IndicatorValueComparison.HigherIsBetter)
            {
                CanBeNormalized = true,
                Comment = "Lower amount is better",
                Description = "Developers that commit to many incidents but doesn't solve them.",
                ValueIdType = ValueIdType.AccountId,
                ValueUnit = "incident(s)"
            };
        }
    }


}
