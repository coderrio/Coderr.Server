using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Modules.Mine.Queries;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Mine
{
    public class ListMyIncidentsQueryHandler : IQueryHandler<ListMyIncidents, ListMyIncidentsResult>
    {
        private readonly IIncidentRepository _repository;
        private readonly IAdoNetUnitOfWork _uow;
        private IApplicationRepository _applicationRepository;

        public ListMyIncidentsQueryHandler(IAdoNetUnitOfWork uow, IIncidentRepository repository, IApplicationRepository applicationRepository)
        {
            _uow = uow;
            _repository = repository;
            _applicationRepository = applicationRepository;
        }

        public async Task<ListMyIncidentsResult> HandleAsync(IMessageContext context, ListMyIncidents query)
        {
            var totalCount = await GetTotalCount(context);
            var incidents = await GetMyItems(context);
            var suggestions = new List<ListMySuggestedItem>();
            var items = await GetMostlyReported(context.Principal.GetAccountId(), query.ApplicationId);
            if (items.Any())
            {
                var enriched = await EnrichSuggestions(items);
                suggestions.AddRange(enriched);
            }

            var comment = "";
            if (incidents.Count == 0)
            {
                if (suggestions.Count > 0)
                {
                    comment = "It's time to do some work. Why don't you start with the selected incident below?";
                }
            }
            else
            {
                var oldestIncident = incidents.OrderByDescending(x => x.DaysOld).First();
                if (oldestIncident.DaysOld > 7)
                {
                    comment =
                        $"Your oldest incident is {oldestIncident.DaysOld} days old. Why don't you solve it before doing something else? <a href=\"/analyze/incident/{oldestIncident.Id}\">Solve it</a>";
                }
                else if (incidents.Count <= 3)
                {
                    comment = $"";
                }
                else if (incidents.Count > 3)
                {
                    var random = new Random();
                    comment = random.Next(0, 2) == 0
                        ? "Did you know that Coderr ignores reports for closed incidents as long as they are for older application versions? Solve some.."
                        : "We recommend that you correct existing incidents before taking new.";
                }
            }

            var result = new ListMyIncidentsResult
            {
                Comment = comment,
                Items = incidents,
                Suggestions = suggestions
            };

            return result;
        }

        private async Task<List<ListMyIncidentsResultItem>> GetMostlyReported(int accountId, int? applicationId)
        {
            var sqlQuery =
                $@"SELECT Incidents.*, Incidents.Description as Name, Applications.Id as ApplicationId, Applications.Name as ApplicationName
                                FROM Incidents 
                                JOIN Applications ON (Applications.Id = Incidents.ApplicationId)
                                WHERE State = {(int)IncidentState.New}";
            if (applicationId == null)
            {
                return await _uow.ToListAsync(new ListMyIncidentsResultItemMapper(), sqlQuery);
                
            }

            sqlQuery += " AND Applications.Id = @appId";
            return await _uow.ToListAsync(new ListMyIncidentsResultItemMapper(), sqlQuery,
                    new { appId = applicationId.Value });
        }

        private async Task<IEnumerable<ListMySuggestedItem>> EnrichSuggestions(List<ListMyIncidentsResultItem> items)
        {
            var incidentIds = items.Select(x => x.Id).Distinct();
            var incidents = await _repository.GetManyAsync(incidentIds);
            var apps = new Dictionary<int, Application>();
            var result = new List<ListMySuggestedItem>();
            foreach (var item in items)
            {
                var incident = incidents.First(x => x.Id == item.Id);
                var suggestion = new ListMySuggestedItem(incident.Id, incident.Description)
                {
                    ApplicationId = incident.ApplicationId,
                    ApplicationName = item.ApplicationName,
                    CreatedAtUtc = incident.CreatedAtUtc,
                    ExceptionTypeName = incident.FullName,
                    LastReportAtUtc = incident.LastReportAtUtc,
                    Motivation = "Frequently reported",
                    StackTrace = incident.StackTrace,
                    ReportCount = incident.ReportCount
                };

                if (suggestion.ApplicationId == 0 || string.IsNullOrEmpty(suggestion.ApplicationName))
                {
                    if (!apps.TryGetValue(suggestion.ApplicationId, out var app))
                    {
                        app = await _applicationRepository.GetByIdAsync(suggestion.ApplicationId);
                        apps[app.Id] = app;
                    }

                    suggestion.ApplicationName = app.Name;
                }
                result.Add(suggestion);
            }

            return result;
        }


        private async Task<List<ListMyIncidentsResultItem>> GetMyItems(IMessageContext context)
        {
            var sqlQuery =
                $@"SELECT Incidents.*, Incidents.Description as Name, Applications.Id as ApplicationId, Applications.Name as ApplicationName
                                FROM Incidents 
                                JOIN Applications ON (Applications.Id = Incidents.ApplicationId)
                                WHERE State = {(int)IncidentState.Active}
                                AND AssignedToId=@userId";

            var incidents =
                await _uow.ToListAsync(new ListMyIncidentsResultItemMapper(), sqlQuery,
                    new { userId = context.Principal.GetAccountId() });
            return incidents;
        }

        private async Task<int> GetTotalCount(IMessageContext context)
        {
            var sqlQuery = $@"SELECT cast(count(*) as int)
FROM Incidents i
JOIN Applications a ON (a.Id = i.ApplicationId)
JOIN ApplicationMembers am ON (a.Id = am.ApplicationId)
WHERE am.ApplicationId = @accountId
";
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.AddParameter("accountId", context.Principal.GetAccountId());
                cmd.CommandText = sqlQuery;
                return (int)await cmd.ExecuteScalarAsync();
            }
        }

    }
}