using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Applications
{
    /// <summary>
    /// Number of solved incidents.
    /// </summary>
    /// <remarks>
    ///<para>
    /// 
    /// </para>
    /// </remarks>
    [ContainerService]
    class SolvedCountIndicator : CounterIndicator
    {

        public SolvedCountIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        private const string IndicatorName = "SolvedIncidentsCount";

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {

            var sql = @"select ApplicationId, cast(FORMAT(ClosedAtUtc,'yyyy-MM-01') as date), count(*), ClosedById
                                    from CommonIncidentProgressTracking
                                    where (ClosedAtUtc >= @startDate AND ClosedAtUtc <= @endDate)
                                    #AppIdConstraint#
                                    group by ApplicationId, cast(FORMAT(ClosedAtUtc,'yyyy-MM-01') as date), ClosedById";
            var items = await ExecuteCountSql(context, sql, true);


            var builderContext = new CountBuilderContext(context, items);
            var systemInsight = CreateIndicator("Corrected incidents");
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);

            systemInsight = CreateIndicator("Have Least corrected incidents", IndicatorValueComparison.HigherIsBetter, true);
            builderContext.SystemCountInsight(systemInsight, x => x.ApplicationId);


            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                var appBuilderContext = new CountBuilderContext(context, filtered);

                var appIndicator1 = CreateIndicator("Corrected incidents");
                appBuilderContext.AppCountInsight(appIndicator1, applicationId, x => x.ApplicationId);
                appIndicator1.ValueIdType = ValueIdType.None;

                var appIndicator2 = CreateIndicator("Solved incidents (per developer)", idType: ValueIdType.AccountId, isVariation:true);
                appBuilderContext.AppCountInsight(appIndicator2, applicationId, x => x.AccountId);

                // Want to use devs as the toplist.
                appIndicator1.Toplist = appIndicator2.Toplist;
                appIndicator1.ToplistValueType = appIndicator2.ToplistValueType;
            }

        }

        private static KeyPerformanceIndicator CreateIndicator(string title, IndicatorValueComparison comparisonType = IndicatorValueComparison.HigherIsBetter, bool isVariation = false, ValueIdType idType = ValueIdType.ApplicationId)
        {
            var kpi = new KeyPerformanceIndicator(IndicatorName, title,
                comparisonType)
            {
                CanBeNormalized = true,
                Comment = "Higher amount is better",
                Description = "Number of incidents that were corrected.",
                IsVariation = isVariation,
                ValueIdType = idType
            };
            return kpi;
        }
    }


}
