using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.Api.Web.Overview;
using OneTrueError.Api.Web.Overview.Queries;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Web.Overview
{
  
    [Component]
    internal class GetOverviewHandler : IQueryHandler<GetOverview, GetOverviewResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetOverviewHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetOverviewResult> ExecuteAsync(GetOverview query)
        {
            if (query.NumberOfDays == 0)
                query.NumberOfDays = 30;

            if (query.NumberOfDays == 1)
                return await GetTodaysOverviewAsync(query);

            var apps = new Dictionary<int, GetOverviewApplicationResult>();
            var labels = new string[query.NumberOfDays+1]; //+1 for today
            var startDate = DateTime.Today.AddDays(-query.NumberOfDays);
            for (int i = 0; i <= query.NumberOfDays; i++) 
            {
                labels[i] = startDate.AddDays(i).ToShortDateString();
            }

            var result = new GetOverviewResult();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText= @"select Applications.Id, Applications.Name, cte.Date, cte.Count
FROM 
(
	select Incidents.ApplicationId , cast(Incidents.CreatedAtUtc as date) as Date, count(Incidents.Id) as Count
	from Incidents
	where Incidents.CreatedAtUtc >= @minDate
	group by Incidents.ApplicationId, cast(Incidents.CreatedAtUtc as date)
) cte
right join applications on (applicationid=applications.id)
;";

               
                cmd.AddParameter("minDate", startDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var appId = reader.GetInt32(0);
                        GetOverviewApplicationResult app;
                        if (!apps.TryGetValue(appId, out app))
                        {
                            app = new GetOverviewApplicationResult(reader.GetString(1), startDate, query.NumberOfDays+1); //+1 for today
                            apps[appId] = app;
                        }
                        //no stats at all for this app
                        if (reader[2] is DBNull)
                        {
                            var startDate2 = DateTime.Today.AddDays(-query.NumberOfDays + 1);
                            for (int i = 0; i < query.NumberOfDays; i++)
                            {
                                app.AddValue( startDate2.AddDays(i),0);
                            }
                        }
                        else
                            app.AddValue(reader.GetDateTime(2), reader.GetInt32(3));
                    }

                    result.TimeAxisLabels = labels;
                    result.IncidentsPerApplication = apps.Values.ToArray();
                }
            }

            await GetStatSummary(query, result);


            return result;
        }

        private async Task GetStatSummary(GetOverview query, GetOverviewResult result)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select count(id) from incidents 
where CreatedAtUtc >= @minDate
AND Incidents.IgnoreReports = 0 
AND Incidents.IsSolved = 0;

select count(id) from errorreports 
where CreatedAtUtc >= @minDate

select count(distinct emailaddress) from IncidentFeedback
where CreatedAtUtc >= @minDate
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0;

select count(*) from IncidentFeedback 
where CreatedAtUtc >= @minDate
AND Description is not null
AND DATALENGTH(Description) > 0;";

                var minDate = query.NumberOfDays == 1
                    ? StartDateForHours
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

        private DateTime StartDateForHours
        {
            get
            {
                //since we want to 22 if time is 21:30
                return DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-22);
            }
        }
        private async Task<GetOverviewResult> GetTodaysOverviewAsync(GetOverview query)
        {
            var result = new GetOverviewResult
            {
                TimeAxisLabels = new string[24]
            };
            var startDate = StartDateForHours;
            var apps = new Dictionary<int, GetOverviewApplicationResult>();
            for (var i = 0; i < 24; i++)
            {
                result.TimeAxisLabels[i] = startDate.AddHours(i).ToString("HH:mm");
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select Applications.Id, Applications.Name, cte.Date, cte.Count
FROM 
(
	select Incidents.ApplicationId , DATEPART(HOUR, Incidents.CreatedAtUtc) as Date, count(Incidents.Id) as Count
	from Incidents
	where Incidents.CreatedAtUtc >= @minDate
	group by Incidents.ApplicationId, DATEPART(HOUR, Incidents.CreatedAtUtc)
) cte
right join applications on (applicationid=applications.id)";


                cmd.AddParameter("minDate", startDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var appId = reader.GetInt32(0);
                        GetOverviewApplicationResult app;
                        if (!apps.TryGetValue(appId, out app))
                        {
                            app = new GetOverviewApplicationResult(reader.GetString(1), startDate, 1);
                            apps[appId] = app;
                        }

                        if (reader[2] is DBNull)
                        {
                            for (var i = 0; i < 24; i++)
                            {
                                app.AddValue(startDate.AddHours(i), 0);
                            }
                        }
                        else
                        {
                            var hour = reader.GetInt32(2);
                            app.AddValue(
                                hour < DateTime.Now.AddHours(1).Hour //since we want 22:00 if time is 21:30
                                    ? DateTime.Today.AddHours(hour)
                                    : DateTime.Today.AddDays(-1).AddHours(hour), reader.GetInt32(3));
                        }
                    }

                    result.IncidentsPerApplication = apps.Values.ToArray();
                }
            }

            await GetStatSummary(query, result);

            return result;
        }
    }
}