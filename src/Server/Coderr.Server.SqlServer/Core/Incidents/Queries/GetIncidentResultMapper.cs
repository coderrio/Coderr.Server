using System;
using System.Collections.Generic;
using codeRR.Server.Api.Core.Incidents.Queries;
using codeRR.Server.App.Core.Incidents;
using codeRR.Server.SqlServer.Tools;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    public class GetIncidentResultMapper : CrudEntityMapper<GetIncidentResult>
    {
        public GetIncidentResultMapper()
            : base("Incidents")
        {
            Property(x => x.DayStatistics).Ignore();
            Property(x => x.SuggestedSolutions).Ignore();
            Property(x => x.Facts).Ignore();
            Property(x => x.HighlightedContextData).Ignore();
            Property(x => x.Tags).Ignore();
            Property(x => x.ContextCollections).Ignore();
            Property(x => x.IncidentState)
                .ColumnName("State");
            Property(x => x.AssignedToId)
                .ToPropertyValue(x => x is DBNull ? (int?) null : (int) x);

            Property(x => x.Solution)
                .ToPropertyValue(x => EntitySerializer.Deserialize<IncidentSolution>(x)?.Description);

            Property(x => x.LastReportReceivedAtUtc)
                .ToPropertyValue(DbConverters.ToEntityDate)
                .ColumnName("LastReportAtUtc");

            Property(x => x.IsSolved).Ignore();
            Property(x => x.IsIgnored).Ignore();

            Property(x => x.IsSolutionShared)
                .ToPropertyValue(DbConverters.BoolFromByteArray);

            
        }

    }
}