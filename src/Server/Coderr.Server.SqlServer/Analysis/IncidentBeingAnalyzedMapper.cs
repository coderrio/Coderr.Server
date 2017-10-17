using System;
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

            Property(x => x.IsSolved)
                .ToPropertyValue(o => Convert.ToBoolean(((byte[]) o)[0]));

            Property(x => x.IsIgnored)
                .ColumnName("IgnoreReports");
        }
    }
}