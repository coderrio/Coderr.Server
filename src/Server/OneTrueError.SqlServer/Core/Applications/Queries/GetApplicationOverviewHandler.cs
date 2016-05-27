using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Applications.Queries
{
    [Component]
    internal class GetApplicationOverviewHandler : IQueryHandler<GetApplicationOverview, GetApplicationOverviewResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetApplicationOverviewHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetApplicationOverviewResult> ExecuteAsync(GetApplicationOverview query)
        {
            if (query.NumberOfDays == 0)
                query.NumberOfDays = 30;

            if (query.NumberOfDays == 1)
                return await GetTodaysOverviewAsync(query);

            var curDate = DateTime.Today.AddDays(-query.NumberOfDays);
            var errorReports = new Dictionary<DateTime, int>();
            var incidents = new Dictionary<DateTime, int>();
            while (curDate <= DateTime.Today)
            {
                errorReports[curDate] = 0;
                incidents[curDate] = 0;
                curDate = curDate.AddDays(1);
            }

            var result = new GetApplicationOverviewResult();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                var sql = @"select cast(Incidents.CreatedAtUtc as date), count(Id)
from Incidents
where Incidents.CreatedAtUtc >= @minDate
{0}
group by cast(Incidents.CreatedAtUtc as date);
select cast(ErrorReports.CreatedAtUtc as date), count(Id)
from ErrorReports
where ErrorReports.CreatedAtUtc >= @minDate
{1}
group by cast(ErrorReports.CreatedAtUtc as date);";

                if (query.ApplicationId > 0)
                {
                    cmd.CommandText = string.Format(sql,
                        " AND Incidents.ApplicationId = @appId",
                        " AND ErrorReports.ApplicationId = @appId");
                    cmd.AddParameter("appId", query.ApplicationId);
                }
                else
                {
                    cmd.CommandText = string.Format(sql, "", "");
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
                    result.TimeAxisLabels = incidents.Select(x => x.Key.ToShortDateString()).ToArray();
                }
            }

            await GetStatSummary(query, result);


            return result;
        }

        private async Task GetStatSummary(GetApplicationOverview query, GetApplicationOverviewResult result)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select count(id) from incidents 
where CreatedAtUtc >= @minDate
AND ApplicationId = @appId 
AND Incidents.IgnoreReports = 0 
AND Incidents.IsSolved = 0;

select count(id) from errorreports 
where CreatedAtUtc >= @minDate
AND ApplicationId = @appId;

select count(distinct emailaddress) from IncidentFeedback
where CreatedAtUtc >= @minDate
AND ApplicationId = @appId
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0;

select count(*) from IncidentFeedback 
where CreatedAtUtc >= @minDate
AND ApplicationId = @appId
AND Description is not null
AND DATALENGTH(Description) > 0;";
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

                    var data = new OverviewStatSummary();
                    data.Incidents = reader.GetInt32(0);
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.Reports = reader.GetInt32(0);
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.Followers = reader.GetInt32(0);
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    data.UserFeedback = reader.GetInt32(0);
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

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                var sql = @"SELECT DATEPART(HOUR, Incidents.CreatedAtUtc), cast(count(Id) as int)
from Incidents
where Incidents.CreatedAtUtc >= @minDate
{0}
group by DATEPART(HOUR, Incidents.CreatedAtUtc);
select DATEPART(HOUR, ErrorReports.CreatedAtUtc), cast(count(Id) as int)
from ErrorReports
where ErrorReports.CreatedAtUtc >= @minDate
{1}
group by DATEPART(HOUR, ErrorReports.CreatedAtUtc);";

                cmd.CommandText = string.Format(sql,
                    " AND Incidents.ApplicationId = @appId",
                    " AND ErrorReports.ApplicationId = @appId");
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