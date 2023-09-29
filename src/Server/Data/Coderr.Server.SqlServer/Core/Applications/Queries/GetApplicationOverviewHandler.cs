using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Core.Applications.Queries
{
    internal class GetApplicationOverviewHandler : IQueryHandler<GetApplicationOverview, GetApplicationOverviewResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetApplicationOverviewHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetApplicationOverviewResult> HandleAsync(IMessageContext context, GetApplicationOverview query)
        {
            if (query.NumberOfDays == 0)
                query.NumberOfDays = 30;

            if (query.NumberOfDays == 1)
                return await GetTodaysOverviewAsync(query);

            var result = new GetApplicationOverviewResult();

            if (query.IncludeChartData)
            {
                await LoadChartData(query, result);
            }
            
            await GetStatSummary(query, result);
            return result;
        }

        private async Task LoadChartData(GetApplicationOverview query, GetApplicationOverviewResult result)
        {
            var curDate = DateTime.Today.AddDays(-query.NumberOfDays);
            var errorReports = new Dictionary<DateTime, int>();
            var incidents = new Dictionary<DateTime, int>();
            while (curDate <= DateTime.Today)
            {
                errorReports[curDate] = 0;
                incidents[curDate] = 0;
                curDate = curDate.AddDays(1);
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                string filter1;
                string filter2;
                if (query.Version != null)
                {
                    var id = _unitOfWork.ExecuteScalar("SELECT Id FROM ApplicationVersions WHERE Version = @version",
                        new {version = query.Version});
                    filter1 = @"JOIN IncidentVersions On (Incidents.Id = IncidentVersions.IncidentId)
                            WHERE IncidentVersions.VersionId = @versionId AND ";
                    filter2 = @"JOIN IncidentVersions On (IncidentReports.IncidentId = IncidentVersions.IncidentId)
                            WHERE IncidentVersions.VersionId = @versionId AND ";
                    cmd.AddParameter("versionId", id);
                }
                else
                {
                    filter1 = "WHERE ";
                    filter2 = "WHERE ";
                }

                var sql = @"select cast(Incidents.CreatedAtUtc as date), count(Incidents.Id)
from Incidents WITH (ReadUncommitted)
{2} Incidents.CreatedAtUtc >= @minDate
AND Incidents.CreatedAtUtc <= GetUtcDate()
{0}
group by cast(Incidents.CreatedAtUtc as date);
select cast(IncidentReports.ReceivedAtUtc as date), count(IncidentReports.Id)
from IncidentReports WITH (ReadUncommitted)
join Incidents isa WITH (ReadUncommitted) ON (isa.Id = IncidentReports.IncidentId)
{3} IncidentReports.ReceivedAtUtc >= @minDate
AND IncidentReports.ReceivedAtUtc <= GetUtcDate()
{1}
group by cast(IncidentReports.ReceivedAtUtc as date);";

                if (query.ApplicationId > 0)
                {
                    cmd.CommandText = string.Format(sql,
                        " AND Incidents.ApplicationId = @appId",
                        " AND isa.ApplicationId = @appId",
                        filter1, filter2);
                    cmd.AddParameter("appId", query.ApplicationId);
                }
                else
                {
                    cmd.CommandText = string.Format(sql, "", "", filter1, filter2);
                }

                cmd.AddParameter("minDate", DateTime.Today.AddDays(-query.NumberOfDays));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        incidents[(DateTime)reader[0]] = (int)reader[1];
                    }

                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        errorReports[(DateTime)reader[0]] = (int)reader[1];
                    }

                    result.ErrorReports = errorReports.Select(x => x.Value).ToArray();
                    result.Incidents = incidents.Select(x => x.Value).ToArray();
                    result.TimeAxisLabels = incidents.Select(x => x.Key.ToString("yyyy-MM-dd")).ToArray();
                }
            }
        }

        private async Task GetStatSummary(GetApplicationOverview query, GetApplicationOverviewResult result)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                if (!string.IsNullOrEmpty(query.Version))
                {
                    var versionId =
                        _unitOfWork.ExecuteScalar("SELECT Id FROM ApplicationVersions WHERE Version=@version",
                            new { version = query.Version });
                    cmd.CommandText = $@"select count(id), max(CreatedAtUtc) 
from incidents  WITH (ReadUncommitted)
JOIN IncidentVersions ON (Incidents.Id = IncidentVersions.IncidentId)
WHERE IncidentVersions.VersionId = @versionId
AND CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId 
AND Incidents.State <> {(int)IncidentState.Ignored}
AND Incidents.State <> {(int)IncidentState.Closed};

SELECT count(id), max(ReceivedAtUtc)
from IncidentReports  WITH (ReadUncommitted)
JOIN IncidentVersions ON (IncidentReports.IncidentId = IncidentVersions.IncidentId)
WHERE IncidentVersions.VersionId = @versionId
AND ReceivedAtUtc >= @minDate
AND ReceivedAtUtc <= GetUtcDate()
AND ApplicationId = @appId;

SELECT count(distinct emailaddress) 
from IncidentFeedback
JOIN IncidentVersions ON (IncidentFeedback.IncidentId = IncidentVersions.IncidentId)
WHERE IncidentVersions.VersionId = @versionId
AND CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0;

select count(*) 
from IncidentFeedback 
JOIN IncidentVersions ON (IncidentFeedback.IncidentId = IncidentVersions.IncidentId)
WHERE IncidentVersions.VersionId = @versionId
AND CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId
AND Description is not null
AND DATALENGTH(Description) > 0;";
                    cmd.AddParameter("versionId", versionId);
                }
                else
                {
                    cmd.CommandText = $@"select count(id), max(CreatedAtUtc) 
from incidents  WITH (ReadUncommitted)
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId 
AND Incidents.State <> {(int)IncidentState.Ignored}
AND Incidents.State <> {(int)IncidentState.Closed};

SELECT count(IncidentReports.id), max(ReceivedAtUtc)
FROM IncidentReports WITH (ReadUncommitted)
JOIN Incidents ON (Incidents.Id = IncidentReports.IncidentId)
WHERE ReceivedAtUtc >= @minDate
AND ReceivedAtUtc <= GetUtcDate()
AND ApplicationId = @appId;

select count(distinct emailaddress) 
from IncidentFeedback
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0;

select count(*) 
from IncidentFeedback 
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId = @appId
AND Description is not null
AND DATALENGTH(Description) > 0;";

                }

                if (query.IncludePartitions)
                {
                    cmd.CommandText += @"
                        select max(pd.Name), max(pd.PartitionKey), partitionid, count(distinct value)
                        from ApplicationPartitionInsights  api
                        join PartitionDefinitions pd on (pd.Id = api.PartitionId)
                        where YearMonth = @yearMonth
                        group by partitionId";
                    cmd.AddParameter("yearMonth", new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
                }

                cmd.AddParameter("appId", query.ApplicationId);
                var minDate = query.NumberOfDays == 1
                    ? DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-23)
                    : DateTime.Today.AddDays(-query.NumberOfDays);
                cmd.AddParameter("minDate", minDate);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                    {
                        throw new InvalidOperationException("Expected to be able to read.");
                    }

                    var value = reader[1];
                    var data = new OverviewStatSummary
                    {
                        Incidents = reader.GetInt32(0),
                        NewestIncidentReceivedAtUtc = value is DBNull ? null : (DateTime?)value
                    };

                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.Reports = reader.GetInt32(0);
                    value = reader[1];
                    data.NewestReportReceivedAtUtc = value is DBNull ? null : (DateTime?)value;

                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.Followers = reader.GetInt32(0);
                    
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.UserFeedback = reader.GetInt32(0);

                    if (query.IncludePartitions && ServerConfig.Instance.IsCommercial)
                    {
                        await reader.NextResultAsync();
                        var partitions = new List<PartitionOverview>();
                        while (await reader.ReadAsync())
                        {
                            var item = new PartitionOverview
                            {
                                Name = reader.GetString(1),
                                DisplayName = reader.GetString(0),
                                Value = reader.GetInt32(3)
                            };
                            partitions.Add(item);
                        }

                        data.Partitions = partitions.ToArray();
                    }

                    result.StatSummary = data;
                }
            }
        }

        private async Task<GetApplicationOverviewResult> GetTodaysOverviewAsync(GetApplicationOverview query)
        {
            var result = new GetApplicationOverviewResult
            {
                TimeAxisLabels = new string[24]
            };
            var incidentValues = new Dictionary<DateTime, int>();
            var reportValues = new Dictionary<DateTime, int>();

            var startDate = DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-23);
            for (var i = 0; i < 24; i++)
            {
                result.TimeAxisLabels[i] = startDate.AddHours(i).ToString("HH:mm");
                incidentValues[startDate.AddHours(i)] = 0;
                reportValues[startDate.AddHours(i)] = 0;
            }
            var filter1 = "";
            var filter2 = "";
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                if (query.Version != null)
                {
                    var id = _unitOfWork.ExecuteScalar("SELECT Id FROM ApplicationVersions WHERE Version = @version",
                        new { version = query.Version });
                    filter1 = @"JOIN IncidentVersions On (Incidents.Id = IncidentVersions.IncidentId)
                            WHERE IncidentVersions.VersionId = @versionId AND ";
                    filter2 = @"JOIN IncidentVersions On (IncidentReports.IncidentId = IncidentVersions.IncidentId)
                            WHERE IncidentVersions.VersionId = @versionId AND ";
                    cmd.AddParameter("versionId", id);
                }
                else
                {
                    filter1 = "WHERE ";
                    filter2 = "WHERE ";
                }

                var sql = @"SELECT DATEPART(HOUR, Incidents.CreatedAtUtc), cast(count(Id) as int)
 from Incidents WITH (ReadUncommitted)
 {0} Incidents.CreatedAtUtc >= @minDate
 AND Incidents.CreatedAtUtc <= GetUtcDate()
 AND Incidents.ApplicationId = @appId
 group by DATEPART(HOUR, Incidents.CreatedAtUtc);
 select DATEPART(HOUR, IncidentReports.ReceivedAtUtc), cast(count(Id) as int)
 from IncidentReports WITH (ReadUncommitted)
 JOIN Incidents ice ON (ice.Id = IncidentId)
 {1} IncidentReports.ReceivedAtUtc >= @minDate
 AND IncidentReports.ReceivedAtUtc <= GetUtcDate()
 AND ice.ApplicationId = @appId
 group by DATEPART(HOUR, IncidentReports.ReceivedAtUtc);";

                cmd.CommandText = string.Format(sql, filter1, filter2);
                cmd.AddParameter("appId", query.ApplicationId);
                cmd.AddParameter("minDate", startDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var todayWithHour = DateTime.Today.AddHours(DateTime.Now.Hour);
                    while (await reader.ReadAsync())
                    {
                        var hour = reader.GetInt32(0);
                        var date = hour < todayWithHour.Hour
                            ? DateTime.Today.AddHours(hour)
                            : DateTime.Today.AddDays(-1).AddHours(hour);
                        incidentValues[date] = reader.GetInt32(1);
                    }
                    await reader.NextResultAsync();
                    while (await reader.ReadAsync())
                    {
                        var hour = reader.GetInt32(0);
                        var date = hour < todayWithHour.Hour
                            ? DateTime.Today.AddHours(hour)
                            : DateTime.Today.AddDays(-1).AddHours(hour);
                        reportValues[date] = reader.GetInt32(1);
                    }
                }
            }

            result.ErrorReports = reportValues.Values.ToArray();
            result.Incidents = incidentValues.Values.ToArray();

            //a bit weird, but required since the method
            await GetStatSummary(query, result);

            return result;
        }
    }
}