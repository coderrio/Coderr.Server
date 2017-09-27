using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    [Component]
    public class GetIncidentHandler : IQueryHandler<GetIncident, GetIncidentResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetIncidentResult> ExecuteAsync(GetIncident query)
        {
            var result = await _unitOfWork.FirstAsync<GetIncidentResult>(new {Id = query.IncidentId});

            var tags = GetTags(query.IncidentId);
            result.Tags = tags.ToArray();

            await GetContextCollectionNames(result);
            await GetReportStatistics(result);
            await GetStatSummary(query, result);
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

        private async Task GetStatSummary(GetIncident query, GetIncidentResult result)
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
                    {
                        throw new InvalidOperationException("Expected to be able to read result 1.");
                    }

                    result.WaitingUserCount = reader.GetInt32(0);

                    await reader.NextResultAsync();
                    if (!await reader.ReadAsync())
                    {
                        throw new InvalidOperationException("Expected to be able to read result 2.");
                    }

                    result.FeedbackCount = reader.GetInt32(0);
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