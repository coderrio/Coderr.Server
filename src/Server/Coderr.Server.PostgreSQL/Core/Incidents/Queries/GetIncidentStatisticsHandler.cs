using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;

namespace Coderr.Server.PostgreSQL.Core.Incidents.Queries
{
    internal class GetIncidentStatisticsHandler : IQueryHandler<GetIncidentStatistics, GetIncidentStatisticsResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentStatisticsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException("unitOfWork");
            _unitOfWork = unitOfWork;
        }

        public async Task<GetIncidentStatisticsResult> HandleAsync(IMessageContext context, GetIncidentStatistics query)
        {
            if (query.NumberOfDays == 1)
                return await GetTodaysOverviewAsync(query);

            var result = new GetIncidentStatisticsResult
            {
                Labels = new string[query.NumberOfDays],
                Values = new int[query.NumberOfDays]
            };

            var startDate = DateTime.Today.AddDays(-query.NumberOfDays + 1);
            for (var i = 0; i < query.NumberOfDays; i++)
            {
                result.Values[i] = 0;
                result.Labels[i] = startDate.AddDays(i).ToShortDateString();
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select cast(ReceivedAtUtc as date) as Date, count(*) as Count
from IncidentReports
where incidentid=@id
AND ReceivedAtUtc > @date
group by cast(ReceivedAtUtc as date);";
                cmd.AddParameter("id", query.IncidentId);
                cmd.AddParameter("date", DateTime.Today.AddDays(0 - query.NumberOfDays));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var index = reader.GetDateTime(0).Subtract(startDate).Days;
                        result.Values[index] = reader.GetInt32(1);
                    }
                }
            }

            return result;
        }

        private async Task<GetIncidentStatisticsResult> GetTodaysOverviewAsync(GetIncidentStatistics query)
        {
            var result = new GetIncidentStatisticsResult
            {
                Labels = new string[24],
                Values = new int[24]
            };
            var values = new Dictionary<DateTime, int>();
            var startDate = DateTime.Today.AddHours(DateTime.Now.Hour).AddHours(-23);
            for (var i = 0; i < 24; i++)
            {
                result.Values[i] = 0;
                result.Labels[i] = startDate.AddHours(i).ToString("HH:mm");
                values[startDate.AddHours(i)] = 0;
            }

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT DATEPART(HOUR, IncidentReports.ReceivedAtUtc), cast(count(Id) as int)
FROM IncidentReports
WHERE IncidentReports.ReceivedAtUtc > @minDate
AND IncidentId = @incidentId
GROUP BY DATEPART(HOUR, IncidentReports.ReceivedAtUtc);";


                cmd.AddParameter("incidentId", query.IncidentId);
                cmd.AddParameter("minDate", startDate);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var todayWithHour = DateTime.Today.AddHours(DateTime.Now.Hour);
                        var hour = reader.GetInt32(0);
                        var date = hour < todayWithHour.Hour
                            ? DateTime.Today.AddHours(hour)
                            : DateTime.Today.AddDays(-1).AddHours(hour);
                        values[date] = reader.GetInt32(1);
                    }
                }
            }

            result.Values = values.Values.ToArray();

            return result;
        }
    }
}