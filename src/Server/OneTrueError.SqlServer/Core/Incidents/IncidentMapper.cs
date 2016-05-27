using System;
using System.Collections.Generic;
using Griffin.Data.Mapper;
using OneTrueError.App.Core.Incidents;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Incidents
{
    public class IncidentMapper : CrudEntityMapper<Incident>
    {
        public IncidentMapper() : base("Incidents")
        {
            Property(x => x.SolvedAtUtc)
                .ToColumnValue(DbConverters.ToNullableSqlDate)
                .ToPropertyValue(DbConverters.ToEntityDate);

            Property(x => x.LastSolutionAtUtc)
                .ToColumnValue(DbConverters.ToNullableSqlDate)
                .ToPropertyValue(DbConverters.ToEntityDate);

            Property(x => x.IgnoringReportsSinceUtc)
                .ToColumnValue(DbConverters.ToNullableSqlDate)
                .ToPropertyValue(DbConverters.ToEntityDate);

            Property(x => x.Solution)
                .ToColumnValue(DbConverters.SerializeEntity)
                .ToPropertyValue(EntitySerializer.Deserialize<IncidentSolution>);

            Property(x => x.IsSolved)
                .ToPropertyValue(DbConverters.BoolFromByteArray);

            Property(x => x.IsSolutionShared)
                .ToPropertyValue(DbConverters.BoolFromByteArray);

        }
    }
}