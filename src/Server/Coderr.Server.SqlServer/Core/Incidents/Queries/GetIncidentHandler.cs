using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;
using Coderr.Server.PluginApi;
using Coderr.Server.PluginApi.Incidents;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    [Component]
    public class GetIncidentHandler : IQueryHandler<GetIncident, GetIncidentResult>
    {
        private readonly IEnumerable<IQuickfactProvider> _quickfactProviders;
        private readonly IEnumerable<IHighlightedContextDataProvider> _highlightedContextDataProviders;
        private readonly IEnumerable<ISolutionProvider> _solutionProviders;
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentHandler(IAdoNetUnitOfWork unitOfWork, 
            IEnumerable<IQuickfactProvider> quickfactProviders,
            IEnumerable<IHighlightedContextDataProvider> highlightedContextDataProviders,
            IEnumerable<ISolutionProvider> solutionProviders
            )
        {
            _unitOfWork = unitOfWork;
            _quickfactProviders = quickfactProviders;
            _highlightedContextDataProviders = highlightedContextDataProviders;
            _solutionProviders = solutionProviders;
        }

        public async Task<GetIncidentResult> HandleAsync(IMessageContext context, GetIncident query)
        {
            var sql =
                "SELECT Incidents.*, Users.Username as AssignedTo " +
                " FROM Incidents " +
                " LEFT JOIN Users ON (AssignedToId = Users.AccountId) " +
                " WHERE Incidents.Id = @id";

            var result = await _unitOfWork.FirstAsync<GetIncidentResult>(sql, new {Id = query.IncidentId});

            var tags = GetTags(query.IncidentId);
            result.Tags = tags.ToArray();

            var facts = new List<QuickFact>
            {
                new QuickFact
                {
                    Title = "Report Count",
                    Description = "Number of reports since this incident was discovered",
                    Value = result.ReportCount.ToString()
                }
            };

            await GetContextCollectionNames(result);
            await GetReportStatistics(result);
            await GetStatSummary(query, facts);

            var contextData = new List<HighlightedContextData>();
            var solutions = new List<SuggestedIncidentSolution>();
            foreach (var provider in _quickfactProviders)
            {
                await provider.AssignAsync(query.IncidentId, facts);
            }
            foreach (var provider in _highlightedContextDataProviders)
            {
                await provider.CollectAsync(query.IncidentId, contextData);
            }
            foreach (var provider in _solutionProviders)
            {
                await provider.SuggestSolutionAsync(query.IncidentId, solutions);
            }

            result.Facts = facts.ToArray();
            result.SuggestedSolutions = solutions.ToArray();
            result.HighlightedContextData = contextData.ToArray();
            return result;
        }

        //TODO : Do not mess with the similarity tables directly
        private async Task GetContextCollectionNames(GetIncidentResult result)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select distinct Name
from [IncidentContextCollections]
where IncidentId=@incidentId";
                cmd.AddParameter("incidentId", result.Id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var names = new List<string>();
                    while (await reader.ReadAsync())
                    {
                        names.Add(reader.GetString(0));
                    }
                    result.ContextCollections = names.ToArray();
                }
            }
        }

        private async Task GetReportStatistics(GetIncidentResult result)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"select cast(createdatutc as date) as Date, count(*) as Count
from errorreports
where incidentid=@incidentId
AND CreatedAtUtc > @date
group by cast(createdatutc as date)";
                var startDate = DateTime.Today.AddDays(-29);
                cmd.AddParameter("date", startDate);
                cmd.AddParameter("incidentId", result.Id);
                var specifiedDays = await cmd.ToListAsync<ReportDay>();
                var curDate = startDate;
                var values = new ReportDay[30];
                var valuesIndexer = 0;
                var specifiedDaysIndexer = 0;
                while (curDate <= DateTime.Today)
                {
                    if (specifiedDays.Count > specifiedDaysIndexer &&
                        specifiedDays[specifiedDaysIndexer].Date == curDate)
                        values[valuesIndexer++] = specifiedDays[specifiedDaysIndexer++];
                    else
                        values[valuesIndexer++] = new ReportDay {Date = curDate};
                    curDate = curDate.AddDays(1);
                }
                result.DayStatistics = values;
            }
        }

        private async Task GetStatSummary(GetIncident query, ICollection<QuickFact> facts)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"
select count(distinct emailaddress) from IncidentFeedback
where @minDate < CreatedAtUtc
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0
AND IncidentId = @incidentId;
select count(*) from IncidentFeedback 
where @minDate < CreatedAtUtc
AND Description is not null
AND DATALENGTH(Description) > 0
AND IncidentId = @incidentId;";
                cmd.AddParameter("incidentId", query.IncidentId);
                cmd.AddParameter("minDate", DateTime.Today.AddDays(-90));

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        throw new InvalidOperationException("Expected to be able to read result 1.");

                    facts.Add(new QuickFact
                    {
                        Title = "Subscribed users",
                        Description =
                            "Number of users that are waiting on a notification when the incident have been solved.",
                        Value = reader.GetInt32(0).ToString()
                    });

                    await reader.NextResultAsync();
                    if (!await reader.ReadAsync())
                        throw new InvalidOperationException("Expected to be able to read result 2.");

                    facts.Add(new QuickFact
                    {
                        Title = "Bug reports",
                        Description = "Number of bug reports written by users.",
                        Value = reader.GetInt32(0).ToString()
                    });
                }
            }
        }

        private List<string> GetTags(int incidentId)
        {
            var tags = new List<string>();
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT TagName FROM IncidentTags WHERE IncidentId=@id";
                cmd.AddParameter("id", incidentId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add((string) reader[0]);
                    }
                }
            }
            return tags;
        }
    }
}