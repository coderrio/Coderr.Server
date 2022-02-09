using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Web.Overview.Queries;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Web.Overview
{
    internal class GetOverviewHandler : IQueryHandler<GetOverview, GetOverviewResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetOverviewHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private DateTime StartDateForHours =>
            //since we want to 22 if time is 21:30
            DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-22);

        private string ApplicationIds { get; set; }

        public async Task<GetOverviewResult> HandleAsync(IMessageContext context, GetOverview query)
        {
            if (query.NumberOfDays == 0)
                query.NumberOfDays = 30;

            var isSysAdmin = context.Principal.IsSysAdmin();
            var gotApps = context.Principal.FindAll(x => x.Type == CoderrClaims.Application).Any();

            var labels = query.IncludeChartData ? CreateTimeLabels(query) : new string[0];

            if (!isSysAdmin && !gotApps)
            {
                return new GetOverviewResult()
                {
                    StatSummary = new OverviewStatSummary(),
                    IncidentsPerApplication = new GetOverviewApplicationResult[0],
                    TimeAxisLabels = labels
                };
            }

            AssignApplicationIds(context, isSysAdmin);
            if (!ApplicationIds.Any())
                return new GetOverviewResult();

            if (query.NumberOfDays == 1)
                return await GetTodaysOverviewAsync(query);

            var result = new GetOverviewResult { Days = query.NumberOfDays };
            if (query.IncludeChartData)
            {
                await LoadChartData(query, result, labels);
            }
            

            // Not in live
            //using (var cmd = _unitOfWork.CreateCommand())
            //{
            //    var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            //    var to = DateTime.UtcNow;
            //    cmd.CommandText =
            //        "SELECT sum(NumberOfReports) FROM IgnoredReports WHERE  date >= @from ANd date <= @to";
            //    cmd.AddParameter("from", from);
            //    cmd.AddParameter("to", to);
            //    var value = cmd.ExecuteScalar();
            //    if (value != DBNull.Value)
            //        result.MissedReports = (int) value;

            //}
            
            await GetStatSummary(query, result);


            return result;
        }

        private async Task LoadChartData(GetOverview query, GetOverviewResult result, string[] labels)
        {
            var apps = new Dictionary<int, GetOverviewApplicationResult>();
            var startDate = DateTime.Today.AddDays(-query.NumberOfDays);

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = $@"select Applications.Id, Applications.Name, cte.Date, cte.Count
FROM 
(
	select Incidents.ApplicationId , cast(Incidents.CreatedAtUtc as date) as Date, count(Incidents.Id) as Count
	from Incidents WITH(READUNCOMMITTED)
	where Incidents.CreatedAtUtc >= @minDate 
    AND Incidents.CreatedAtUtc <= GetUtcDate()
	AND Incidents.ApplicationId in ({ApplicationIds})
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
                        if (!apps.TryGetValue(appId, out var app))
                        {
                            app = new GetOverviewApplicationResult(reader.GetString(1), startDate,
                                query.NumberOfDays + 1); //+1 for today
                            apps[appId] = app;
                        }

                        //no stats at all for this app
                        if (reader[2] is DBNull)
                        {
                            var startDate2 = DateTime.Today.AddDays(-query.NumberOfDays + 1);
                            for (var i = 0; i < query.NumberOfDays; i++)
                            {
                                app.AddValue(startDate2.AddDays(i), 0);
                            }
                        }
                        else
                            app.AddValue(reader.GetDateTime(2), reader.GetInt32(3));
                    }

                    result.TimeAxisLabels = labels;
                    result.IncidentsPerApplication = apps.Values.ToArray();
                }
            }
        }

        private void AssignApplicationIds(IMessageContext context, bool isSysAdmin)
        {
            if (isSysAdmin)
            {
                var appIds = new List<int>();
                using (var cmd = _unitOfWork.CreateCommand())
                {
                    cmd.CommandText = "SELECT id FROM Applications WITH(READUNCOMMITTED)";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            appIds.Add(reader.GetInt32(0));
                        }
                    }
                }

                ApplicationIds = string.Join(",", appIds);
            }
            else
            {
                var appIds = context.Principal
                    .FindAll(x => x.Type == CoderrClaims.Application)
                    .Select(x => int.Parse(x.Value).ToString())
                    .ToList();
                ApplicationIds = string.Join(",", appIds);
            }
        }

        private static string[] CreateTimeLabels(GetOverview query)
        {
            var startDate = DateTime.Today.AddDays(-query.NumberOfDays);
            var labels = new string[query.NumberOfDays + 1]; //+1 for today
            for (var i = 0; i <= query.NumberOfDays; i++)
            {
                labels[i] = startDate.AddDays(i).ToString("yyyy-MM-dd");
            }
            return labels;
        }

        private async Task GetStatSummary(GetOverview query, GetOverviewResult result)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = $@"select count(id), max(CreatedAtUtc) 
from incidents With(READUNCOMMITTED)
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND Incidents.ApplicationId IN ({ApplicationIds})
AND Incidents.State <> {(int)IncidentState.Ignored} 
AND Incidents.State <> {(int)IncidentState.Closed};

select count(id), max(ReceivedAtUtc)
from errorreports WITH(READUNCOMMITTED) 
where CreatedAtUtc >= @minDate
AND ApplicationId IN ({ApplicationIds})

select count(distinct emailaddress) 
from IncidentFeedback WITH(READUNCOMMITTED)
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId IN ({ApplicationIds})
AND emailaddress is not null
AND DATALENGTH(emailaddress) > 0;

select count(*) 
from IncidentFeedback WITH(READUNCOMMITTED)
where CreatedAtUtc >= @minDate
AND CreatedAtUtc <= GetUtcDate()
AND ApplicationId IN ({ApplicationIds})
AND Description is not null
AND DATALENGTH(Description) > 0;
";
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

                    var value = reader[1];
                    var data = new OverviewStatSummary
                    {
                        Incidents = reader.GetInt32(0),
                        NewestIncidentReceivedAtUtc = value is DBNull ? null : (DateTime?)value
                    };
                    await reader.NextResultAsync();
                    await reader.ReadAsync();

                    value = reader[1];
                    data.Reports = reader.GetInt32(0);
                    data.NewestReportReceivedAtUtc = value is DBNull ? null : (DateTime?)value;
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    
                    data.Followers = reader.GetInt32(0);
                    await reader.NextResultAsync();
                    await reader.ReadAsync();
                    
                    data.UserFeedback = reader.GetInt32(0);

                    if (query.IncludePartitions)
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
                cmd.CommandText = $@"select Applications.Id, Applications.Name, cte.Date, cte.Count
FROM 
(
	select Incidents.ApplicationId , DATEPART(HOUR, Incidents.CreatedAtUtc) as Date, count(Incidents.Id) as Count
	from Incidents WITH(READUNCOMMITTED)
	where Incidents.CreatedAtUtc >= @minDate
    AND CreatedAtUtc <= GetUtcDate()
    AND Incidents.ApplicationId IN ({ApplicationIds})
	group by Incidents.ApplicationId, DATEPART(HOUR, Incidents.CreatedAtUtc)
) cte
right join applications WITH(READUNCOMMITTED) on (applicationid=applications.id)";


                cmd.AddParameter("minDate", startDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var appId = reader.GetInt32(0);
                        if (!apps.TryGetValue(appId, out var app))
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