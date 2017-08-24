using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Incidents.Queries;
using OneTrueError.App.Core.Incidents;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Incidents.Queries
{
    public class GetIncidentResultMapper : CrudEntityMapper<GetIncidentResult>
    {
        public GetIncidentResultMapper()
            : base("Incidents")
        {
            Property(x => x.DayStatistics).Ignore();
            Property(x => x.WaitingUserCount).Ignore();
            Property(x => x.FeedbackCount).Ignore();
            Property(x => x.Tags).Ignore();
            Property(x => x.ContextCollections).Ignore();
            Property(x => x.IsIgnored).ColumnName("IgnoreReports");

            Property(x => x.Solution)
                .ToPropertyValue(x => EntitySerializer.Deserialize<IncidentSolution>(x)?.Description);

            Property(x => x.IsSolved)
                .ToPropertyValue(DbConverters.BoolFromByteArray);

            Property(x => x.IsSolutionShared)
                .ToPropertyValue(DbConverters.BoolFromByteArray);
        }
    }
}