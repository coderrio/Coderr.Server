using System;
using System.Collections.Generic;
using Griffin.Data.Mapper;
using OneTrueError.ReportAnalyzer.Domain.Incidents;

namespace OneTrueError.SqlServer.Analysis
{
    public class Incident2Mapper : CrudEntityMapper<IncidentBeingAnalyzed>
    {
        public Incident2Mapper()
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