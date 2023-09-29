using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Insights.Users
{
    /// <summary>
    ///     The closer indicator shows the person that closes most incidents per month.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         System
    ///     </para>
    ///     <para>
    ///         The system KPI should show the best person, no matter which application.
    ///     </para>
    ///     <para>
    ///         The trend should show the best person per application.
    ///     </para>
    ///     <para>
    ///         The top list is the best closers no matter which application.
    ///     </para>
    ///     <para>
    ///         Application
    ///     </para>
    ///     <para>
    ///         The KPI should show the best person for the application.
    ///     </para>
    ///     <para>
    ///         The trend should show best persons per month
    ///     </para>
    /// </remarks>
    [ContainerService]
    public class CloserIndicator : CounterIndicator
    {
        private const string TheCloser = "TheCloser";

        public CloserIndicator(IAdoNetUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public override async Task CollectAsync(KeyPerformanceIndicatorContext context)
        {
            var sql =
                @"select ApplicationId, cast(FORMAT(ClosedAtUtc,'yyyy-MM-01') as date) ClosedAt, count(*), ClosedById
                                    from CommonIncidentProgressTracking
                                    where (ClosedAtUtc >= @startDate AND ClosedAtUtc <= @endDate)
                                    #AppIdConstraint#
                                    group by ApplicationId, cast(FORMAT(ClosedAtUtc,'yyyy-MM-01') as date), ClosedById";

            var items = await ExecuteCountSql(context, sql, true);

            var generationContext = new CountBuilderContext(context, items);

            var kpi = CreateKpiBase("The Closer");
            generationContext.SystemCountKeyFigure(kpi, x => x.AccountId);
            generationContext.SystemCountTrend(kpi, x => x.AccountId);
            generationContext.SystemCountTopList(kpi, x => x.AccountId, grouping => grouping.Sum(x => x.Count));

            foreach (var applicationId in context.ApplicationIds)
            {
                var filtered = items.Where(x => x.ApplicationId == applicationId).ToList();
                kpi = CreateKpiBase("The Closer");
                var appContext = new CountBuilderContext(context, filtered);
                appContext.AppCountInsight(kpi, applicationId, x => x.AccountId);
            }
        }

        private KeyPerformanceIndicator CreateKpiBase(string title)
        {
            return new KeyPerformanceIndicator(TheCloser, title,
                IndicatorValueComparison.HigherIsBetter)
            {
                CanBeNormalized = true,
                Comment = "Number of incidents",
                Description = "Coders that solve most incidents",
                ValueIdType = ValueIdType.AccountId,
                ValueUnit = "incident(s)"
            };
        }
    }
}