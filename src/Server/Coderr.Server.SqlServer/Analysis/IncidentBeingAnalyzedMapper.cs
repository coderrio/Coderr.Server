using System;
using codeRR.Server.App.Core.Incidents;
using codeRR.Server.ReportAnalyzer.Domain.Incidents;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Analysis
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

            Property(x => x.IsClosed)
                .Ignore();

            Property(x => x.IsIgnored)
                .Ignore();

            Property(x => x.State)
                .ToPropertyValue(x => (IncidentState) x)
                .ToColumnValue(x => (int) x);
        }
    }
}