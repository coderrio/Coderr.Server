using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
{
    public class GetIncidentHandler : IQueryHandler<GetIncident, GetIncidentResult>
    {
        private readonly IEnumerable<IQuickfactProvider> _quickfactProviders;
        private readonly IEnumerable<IHighlightedContextDataProvider> _highlightedContextDataProviders;
        private readonly IEnumerable<ISolutionProvider> _solutionProviders;
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private ILog _logger = LogManager.GetLogger(typeof(GetIncidentHandler));

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
            _logger.Info("GetIncident step 1");
            var sql =
                "SELECT Incidents.*, Users.Username as AssignedTo " +
                " FROM Incidents WITH(READUNCOMMITTED)" +
                " LEFT JOIN Users WITH(READUNCOMMITTED) ON (AssignedToId = Users.AccountId) " +
                " WHERE Incidents.Id = @id";

            var result = await _unitOfWork.FirstAsync<GetIncidentResult>(sql, new { Id = query.IncidentId });
            _logger.Info("GetIncident step 2");

            result.Tags = GetTags(query.IncidentId);
            _logger.Info("GetIncident step 3");

            var facts = new List<QuickFact>
            {
                new QuickFact
                {
                    Title = "Created",
                    Description = "When we received the first error report",
                    Value = result.CreatedAtUtc.ToShortDateString()
                },
                new QuickFact
                {
                    Title = "Last report",
                    Description = "When we received the most recent error report",
                    Value = result.LastReportReceivedAtUtc.ToShortDateString()
                },
                new QuickFact
                {
                    Title = "Report Count",
                    Description = "Number of reports since this incident was discovered",
                    Value = result.ReportCount.ToString()
                }
            };

            var environments = GetEnvironments(query.IncidentId);
            if (environments.Any())
            {
                facts.Add(new QuickFact
                {
                    Title = "Environments",
                    Value = string.Join(", ", environments)
                });
            }

            _logger.Info("GetIncident step 4");
            await GetContextCollectionNames(result);
            _logger.Info("GetIncident step 5");
            await GetReportStatistics(result);
            _logger.Info("GetIncident step 6");
            await GetStatSummary(query, facts);
            _logger.Info("GetIncident step 7");

            var solutions = new List<SuggestedIncidentSolution>();
            var suggestedSolutionContext = new SolutionProviderContext(solutions)
            {
                ApplicationId = result.ApplicationId,
                Description = result.Description,
                FullName = result.FullName,
                IncidentId = result.Id,
                StackTrace = result.StackTrace,
                Tags = result.Tags
            };

            var contextData = new List<HighlightedContextData>();
            var highlightedContext = new HighlightedContextDataProviderContext(contextData)
            {
                ApplicationId = result.ApplicationId,
                Description = result.Description,
                FullName = result.FullName,
                IncidentId = result.Id,
                StackTrace = result.StackTrace,
                Tags = result.Tags
            };
            var quickFactContext = new QuickFactContext(result.ApplicationId, query.IncidentId, facts);
            foreach (var provider in _quickfactProviders)
            {
                await provider.CollectAsync(quickFactContext);
            }
            foreach (var provider in _highlightedContextDataProviders)
            {
                await provider.CollectAsync(highlightedContext);
            }
            foreach (var provider in _solutionProviders)
            {
                await provider.SuggestSolutionAsync(suggestedSolutionContext);
            }
            _logger.Info("GetIncident step 8");

            result.RelatedIncidents = await GetRelatedIncidents(query.IncidentId);
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
from [IncidentContextCollections] WITH(READUNCOMMITTED)
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

        private async Task<RelatedIncident[]> GetRelatedIncidents(int incidentId)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"with cte (IncidentId)
                                    AS
                                    (
                                      select distinct ic1.IncidentId
                                      FROM IncidentCorrelations ic1 
                                      JOIN IncidentCorrelations ic2 ON (ic1.CorrelationId = ic2.CorrelationId)
                                      WHERE ic2.IncidentId = @id and ic1.IncidentId <> @id
                                    )

                                    select i.Id IncidentId, i.Description Title, i.CreatedAtUtc, i.ApplicationId, a.Name ApplicationName
                                    FROM Incidents i
                                    JOIN Applications a ON (a.Id = i.ApplicationId)
                                    JOIN CTE ON (IncidentId = i.Id)";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var result = new List<RelatedIncident>();
                    while (await reader.ReadAsync())
                    {
                        var item = new RelatedIncident
                        {
                            IncidentId = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            CreatedAtUtc = reader.GetDateTime(2),
                            ApplicationId = reader.GetInt32(3),
                            ApplicationName = reader.GetString(4)
                        };
                        result.Add(item);
                    }

                    return result.ToArray();
                }
            }
        }

        private async Task GetReportStatistics(GetIncidentResult result)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"select cast(createdatutc as date) as Date, count(*) as Count
from errorreports WITH(READUNCOMMITTED)
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
                        values[valuesIndexer++] = new ReportDay { Date = curDate };
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
select count(distinct emailaddress) 
from IncidentFeedback 
where @minDate < CreatedAtUtc
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0
AND IncidentId = @incidentId;

select count(*) 
from IncidentFeedback 
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

        private string[] GetTags(int incidentId)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"Declare @Tags AS Nvarchar(MAX);
                                    SELECT @Tags = COALESCE(@Tags + ';', '') + TagName 
                                    FROM IncidentTags WITH(READUNCOMMITTED)
                                    WHERE IncidentId=@id
                                    ORDER BY OrderNumber, TagName
                                    SELECT @Tags";
                cmd.AddParameter("id", incidentId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return new string[0];

                    var value = reader[0];
                    return value is DBNull
                        ? new string[0]
                        : ((string)value).Split(';');
                }
            }
        }
        private string[] GetEnvironments(int incidentId)
        {
            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"Declare @Names AS Nvarchar(MAX);
                                    SELECT @Names = COALESCE(@Names + ';', '') + Name
                                    FROM IncidentEnvironments ie WITH(READUNCOMMITTED)
                                    JOIN Environments ae WITH(READUNCOMMITTED) ON (ae.Id = ie.EnvironmentId) 
                                    WHERE IncidentId=@id
                                    ORDER BY Name
                                    SELECT @Names";
                cmd.AddParameter("id", incidentId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return new string[0];

                    var value = reader[0];
                    return value is DBNull
                        ? new string[0]
                        : ((string)value).Split(';');
                }
            }
        }
    }
}