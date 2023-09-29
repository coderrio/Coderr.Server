using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Applications
{
    [ContainerService]
    class AssignedCountIndicator : CounterIndicator
    {
        public AssignedCountIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "AssignedCount";
        private const string Comment = "Having a low amount of open incidents are great as those who are assigned are also solved.";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var sql = @"select ApplicationId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date), count(*), AssignedToId
                                    from CommonIncidentProgressTracking
                                    where (AssignedAtUtc >= @startDate AND AssignedAtUtc <= @endDate)
                                    #AppIdConstraint#
                                    group by ApplicationId, cast(FORMAT(AssignedAtUtc,'yyyy-MM-01') as date), AssignedToId";
            var items = await base.ExecuteCountSql(context, sql, true);

            var builderContext = new CountBuilderContext(context, items);
            var systemInsight = CreateIndicator("Assigned incidents");
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

            systemInsight = CreateIndicator("Have most assigned incidents", IndicatorValueComparison.HigherIsBetter, true);
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appBuilderContext = new CountBuilderContext(context, filtered);

                var appIndicator1 = CreateIndicator("Assigned incidents");
                appBuilderContext.AppCountInsight(appIndicator1, applicationId, x => x.ApplicationId);
                appIndicator1.ValueIdType = ValueIdType.None;

                var appIndicator2 = CreateIndicator("Assigned incidents (per developer)", idType: ValueIdType.AccountId, isVariation: true);
                appBuilderContext.AppCountInsight(appIndicator2, applicationId, x => x.AccountId);

                // Want to use devs as the toplist.
                appIndicator1.Toplist = appIndicator2.Toplist;
                appIndicator1.ToplistValueType = appIndicator2.ToplistValueType;
            }

        }

        private KeyPerformanceIndicator CreateIndicator(string title, IndicatorValueComparison comparisonType = IndicatorValueComparison.LowerIsBetter, bool isVariation = false, ValueIdType idType = ValueIdType.ApplicationId)
        {
            return new KeyPerformanceIndicator(IndicatorName, title,
                comparisonType)
            {
                CanBeNormalized = true,
                Comment = Comment,
                Description = "Number of incidents that are being corrected.",
                IsVariation = isVariation,
                ValueIdType = idType
            };
        }
    }


}
