using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;
using Griffin.ApplicationServices;
using Griffin.Data;
using Newtonsoft.Json;

namespace Coderr.Server.App.Modules.MonthlyStats
{
    [ContainerService(RegisterAsSelf = true)]
    internal class CollectStatsJob : IBackgroundJobAsync
    {
        private readonly IConfiguration<CoderrConfigSection> _reportConfiguration;
        private readonly IConfiguration<UsageStatsSettings> _config;
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private static DateTime _reportDate = DateTime.MinValue;

        public CollectStatsJob(IAdoNetUnitOfWork unitOfWork, IConfiguration<UsageStatsSettings> config,
            IConfiguration<CoderrConfigSection> reportConfiguration)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _reportConfiguration = reportConfiguration;
        }

        public async Task ExecuteAsync()
        {
            var lastMonthDate = DateTime.Today.AddMonths(-1);
            if (_reportDate == lastMonthDate)
                return;

            var lastMonth = new DateTime(lastMonthDate.Year, lastMonthDate.Month, 1);
            if (_config.Value.LatestUploadedMonth == null)
            {
                await ReportAllFoundMonths(lastMonth);
                return;
            }

            if (_config.Value?.LatestUploadedMonth == lastMonth)
                return;


            await ReportMonth(lastMonth);
        }

        private async Task<AppResult[]> GetIncidentCounts(DateTime lastMonth)
        {
            var results = new List<AppResult>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select Incidents.ApplicationId, count(incidents.Id)
                                    from incidents
                                    where Incidents.CreatedAtUtc >= @fromDate AND Incidents.CreatedAtUtc < @toDate
                                    group by Incidents.ApplicationId";
                cmd.AddParameter("fromDate", lastMonth);
                cmd.AddParameter("toDate", lastMonth.AddMonths(1));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new AppResult
                        {
                            ApplicationId = reader.GetInt32(0),
                            Count = reader.GetInt32(1),

                        };
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        private async Task<AppResult[]> GetClosedCount(DateTime lastMonth)
        {
            var results = new List<AppResult>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT Incidents.ApplicationId, count(*)
                                    FROM IncidentHistory
                                    JOIN Incidents on (Incidents.Id = IncidentId)
                                    WHERE IncidentHistory.state = 3
                                    AND IncidentHistory.CreatedAtUtc >= @fromDate AND IncidentHistory.CreatedAtUtc < @toDate
                                    GROUP BY Incidents.ApplicationId";
                cmd.AddParameter("fromDate", lastMonth);
                cmd.AddParameter("toDate", lastMonth.AddMonths(1));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new AppResult
                        {
                            ApplicationId = reader.GetInt32(0),
                            Count = reader.GetInt32(1)
                        };
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        private async Task<AppResult[]> GetReOpened(DateTime lastMonth)
        {
            var results = new List<AppResult>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT Incidents.ApplicationId, count(*)
                                    FROM IncidentHistory
                                    JOIN Incidents on (Incidents.Id = IncidentId)
                                    WHERE IncidentHistory.state = 4
                                    AND IncidentHistory.CreatedAtUtc >= @fromDate AND IncidentHistory.CreatedAtUtc < @toDate
                                    GROUP BY Incidents.ApplicationId";
                cmd.AddParameter("fromDate", lastMonth);
                cmd.AddParameter("toDate", lastMonth.AddMonths(1));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new AppResult
                        {
                            ApplicationId = reader.GetInt32(0),
                            Count = reader.GetInt32(1)
                        };
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        private async Task<AppResult[]> GetIgnoredCount(DateTime lastMonth)
        {
            var results = new List<AppResult>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT Incidents.ApplicationId, count(*)
                                    FROM IncidentHistory
                                    JOIN Incidents on (Incidents.Id = IncidentId)
                                    WHERE IncidentHistory.state = 2
                                    AND IncidentHistory.CreatedAtUtc >= @fromDate AND IncidentHistory.CreatedAtUtc < @toDate
                                    GROUP BY Incidents.ApplicationId";
                cmd.AddParameter("fromDate", lastMonth);
                cmd.AddParameter("toDate", lastMonth.AddMonths(1));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new AppResult
                        {
                            ApplicationId = reader.GetInt32(0),
                            Count = reader.GetInt32(1)
                        };
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        private async Task<AppResult[]> GetReportCounts(DateTime lastMonth)
        {
            var results = new List<AppResult>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT ApplicationId, count(*)
                                    FROM IncidentReports
                                    JOIN Incidents ON (Incidents.Id = IncidentId)
                                    where IncidentReports.ReceivedAtUtc >= @fromDate 
                                        AND IncidentReports.ReceivedAtUtc < @toDate
                                    group by ApplicationId";
                cmd.AddParameter("fromDate", lastMonth);
                cmd.AddParameter("toDate", lastMonth.AddMonths(1));
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var item = new AppResult
                        {
                            ApplicationId = reader.GetInt32(0),
                            Count = reader.GetInt32(1)
                        };
                        results.Add(item);
                    }
                }
            }

            return results.ToArray();
        }

        private async Task ReportAllFoundMonths(DateTime lastMonth)
        {
            while (true)
            {
                var result = await ReportMonth(lastMonth);
                if (!result)
                    break;

                lastMonth = lastMonth.AddMonths(-1);
            }
        }

        private async Task<bool> ReportMonth(DateTime lastMonth)
        {
            var apps = new Dictionary<int, ApplicationUsageStatisticsDto>();

            await GetApps(apps);

            var values = await GetIncidentCounts(lastMonth);
            MergeStats(values, apps, (stat, value) => stat.IncidentCount = value);

            values = await GetReportCounts(lastMonth);
            MergeStats(values, apps, (stat, value) => stat.ReportCount = value);

            values = await GetClosedCount(lastMonth);
            MergeStats(values, apps, (stat, value) => stat.ClosedCount = value);

            values = await GetReOpened(lastMonth);
            MergeStats(values, apps, (stat, value) => stat.ReOpenedCount = value);

            values = await GetIgnoredCount(lastMonth);
            MergeStats(values, apps, (stat, value) => stat.IgnoredCount = value);

            if (_config.Value.LatestUploadedMonth == null || _config.Value.LatestUploadedMonth < lastMonth)
            {
                _config.Value.LatestUploadedMonth = lastMonth;
                _reportDate = lastMonth;
                _config.Save();
            }

            var allIsEmpty = apps.All(x => x.Value.IsEmpty);
            if (allIsEmpty)
                return false;

            var dto = new UsageStatisticsDto
            {
                InstallationId = _reportConfiguration.Value.InstallationId,
                Applications = apps.Values.ToArray(),
                YearMonth = lastMonth
            };
            var json = JsonConvert.SerializeObject(dto);
            var client = new HttpClient();
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            //await client.PostAsync("https://coderr.io/stats/usage", content);
            return true;
        }

        private async Task GetApps(Dictionary<int, ApplicationUsageStatisticsDto> apps)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select Id, EstimatedNumberOfErrors, NumberOfFtes FROM Applications";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var estimatedNumberOfErrors = reader[1];
                        var numberOfDevelopers = reader[2];
                        var item = new ApplicationUsageStatisticsDto()
                        {
                            ApplicationId = reader.GetInt32(0),
                            EstimatedNumberOfErrors =
                                estimatedNumberOfErrors is DBNull ? 0 : (int) estimatedNumberOfErrors,
                            NumberOfDevelopers = numberOfDevelopers is DBNull ? 0 : (decimal) numberOfDevelopers
                        };
                        apps[item.ApplicationId] = item;
                    }
                }
            }
        }

        private static void MergeStats(IEnumerable<AppResult> appResults, IDictionary<int, ApplicationUsageStatisticsDto> apps, Action<ApplicationUsageStatisticsDto, int> assignMethod)
        {
            foreach (var count in appResults)
            {
                if (count.Count == 0)
                    continue;
                if (!apps.TryGetValue(count.ApplicationId, out var value))
                {
                    value = new ApplicationUsageStatisticsDto
                    {
                        ApplicationId = count.ApplicationId
                    };
                    apps[value.ApplicationId] = value;
                }

                assignMethod(value, count.Count);
            }
        }
    }
}