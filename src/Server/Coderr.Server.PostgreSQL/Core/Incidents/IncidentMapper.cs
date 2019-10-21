using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.PostgreSQL.Tools;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Incidents
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

            Property(x => x.ReopenedAtUtc)
                .ToColumnValue(DbConverters.ToNullableSqlDate)
                .ToPropertyValue(DbConverters.ToEntityDate);

            Property(x => x.Solution)
                .ToColumnValue(DbConverters.SerializeEntity)
                .ToPropertyValue(EntitySerializer.Deserialize<IncidentSolution>);

            Property(x => x.State);
                

            Property(x => x.IsSolutionShared)
                .ToPropertyValue(DbConverters.BoolFromByteArray);
        }
    }
}