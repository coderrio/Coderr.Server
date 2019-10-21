using Coderr.Server.ReportAnalyzer.Incidents;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.ReportAnalyzer
{
    public class IncidentBeingAnalyzedMapper : CrudEntityMapper<IncidentBeingAnalyzed>
    {
        public IncidentBeingAnalyzedMapper()
            : base("Incidents")
        {
            Property(x => x.UpdatedAtUtc)
                .ToPropertyValue(DbConverters.ToEntityDate)
                .ToColumnValue(DbConverters.ToNullableSqlDate);

            Property(x => x.PreviousSolutionAtUtc)
                .ToPropertyValue(DbConverters.ToEntityDate)
                .ToColumnValue(DbConverters.ToNullableSqlDate);

            Property(x => x.ReOpenedAtUtc)
                .ToPropertyValue(DbConverters.ToEntityDate)
                .ToColumnValue(DbConverters.ToNullableSqlDate);

            Property(x => x.SolvedAtUtc)
                .ToPropertyValue(DbConverters.ToEntityDate)
                .ToColumnValue(DbConverters.ToNullableSqlDate);

            Property(x => x.EnvironmentNames).Ignore();

            Property(x => x.IsClosed)
                .Ignore();

            Property(x => x.IsIgnored)
                .Ignore();

            Property(x => x.State)
                .ToPropertyValue(x => (AnalyzedIncidentState) x)
                .ToColumnValue(x => (int) x);
        }
    }
}