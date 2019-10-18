using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Applications;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Api.Core.Environments.Queries;
using Coderr.Server.Api.Core.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Api.Core.Reports.Queries;
using DotNetCqs;

namespace Coderr.IntegrationTests.Core.Tools
{
    class ReportingApiClient
    {
        private ServerApiClient _client;

        public ReportingApiClient(Uri uri, string apiKey, string sharedSecret)
        {
            _client = new ServerApiClient();
            _client.Open(uri, apiKey, sharedSecret);
        }

        public async Task Reset(int applicationId, string environmentName)
        {
            var envs = new GetEnvironments();
            var result = await _client.QueryAsync(envs);
            var id = result.Items.FirstOrDefault(x => x.Name == environmentName)?.Id;

            // haven't reported for that environment yet
            if (id == null)
                return;

            var cmd = new Coderr.Server.Api.Core.Environments.Commands.ResetEnvironment(applicationId, id.Value);
            await _client.SendAsync(cmd);
        }

        public async Task<bool> CheckIfReportExists(int applicationId, string genericMessage, string reportDetails)
        {
            var report = await GetReportAsync(applicationId, genericMessage, reportDetails);
            return report != null;
        }

        public async Task<GetReportResult> GetReport(int applicationId, string genericMessage, string reportDetails)
        {
            var reportListItem = await GetReportAsync(applicationId, genericMessage, reportDetails);
            if (reportListItem == null)
                throw new InvalidOperationException("Failed to find our uploaded report");

            var query3 = new GetReport(reportListItem.Id);
            return await _client.QueryAsync(query3);
        }

        public async Task<GetIncidentResult> GetIncident(int applicationId, string incidentMessage, string reportMessage = null)
        {
            var id = await GetIncidentId(applicationId, incidentMessage, reportMessage);
            if (id == null)
                throw new InvalidOperationException("Failed to find our uploaded report");


            var query = new GetIncident(id.Value);
            return await _client.QueryAsync(query);
        }

        private async Task<GetReportListResultItem> GetReportAsync(int applicationId, string genericMessage, string reportDetails)
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var query = new FindIncidents
                {
                    ApplicationIds = new[] { applicationId },
                    FreeText = genericMessage,
                    SortType = IncidentOrder.Newest
                };
                var result = await _client.QueryAsync(query);
                if (result.Items.Count > 0)
                {
                    var item = result.Items.FirstOrDefault(x => x.Name.Contains(reportDetails));
                    if (item != null)
                    {
                        var query2 = new GetReportList(item.Id);
                        var result2 = await _client.QueryAsync(query2);
                        var report = result2.Items.FirstOrDefault(x => x.Message.Contains(reportDetails));
                        if (report != null)
                            return report;
                    }

                    foreach (var resultItem in result.Items)
                    {
                        var query2 = new GetReportList(resultItem.Id);
                        var result2 = await _client.QueryAsync(query2);
                        var report = result2.Items.FirstOrDefault(x => x.Message.Contains(reportDetails));
                        if (report != null)
                            return report;
                    }
                }


                await Task.Delay((3 - triesLeft) * 2000);

            }
            return null;
        }

        private async Task<TResult> TryQuery<TQuery, TResult>(TQuery query) where TQuery : Query<TResult>
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var result = await _client.QueryAsync(query);
                if (result != null)
                {
                    var collection = (IList)result.GetType().GetProperty("Items")?.GetValue(result);
                    if (collection != null)
                    {
                        if (collection.Count > 0)
                            return result;
                    }

                    var prop = result.GetType().GetProperties().FirstOrDefault(x => x.PropertyType.GetProperty("Count") != null);
                    if (prop != null)
                    {
                        var collection2 = prop.GetValue(result);
                        var value = (int)collection2.GetType().GetProperty("Count").GetValue(collection2);
                        if (value > 0)
                            return result;
                    }
                }

                await Task.Delay((3 - triesLeft) * 2000);
            }

            return default;
        }

        private async Task Repeat(Func<Task<bool>> logic)
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                if (await logic())
                    break;

                await Task.Delay((3 - triesLeft) * 2000);
            }
        }

        private async Task<int?> GetIncidentId(int applicationId, string incidentMessage, string reportMessage = null)
        {
            int? returnValue = null;
            await Repeat(async () =>
            {
                var query = new FindIncidents
                {
                    ApplicationIds = new[] { applicationId },
                    FreeText = incidentMessage,
                    SortType = IncidentOrder.Newest
                };
                var result = await _client.QueryAsync(query);
                if (result.Items.Count <= 0)
                    return false;

                if (reportMessage == null)
                {
                    returnValue = result.Items[0].Id;
                    return true;
                }

                var item = result.Items.FirstOrDefault(x => x.Name.Contains(reportMessage));
                if (item != null)
                {
                    var query2 = new GetReportList(item.Id);
                    var result2 = await _client.QueryAsync(query2);
                    var report = result2.Items.FirstOrDefault(x => x.Message.Contains(reportMessage));
                    if (report != null)
                    {
                        returnValue = item.Id;
                        return true;
                    }
                }

                foreach (var resultItem in result.Items)
                {
                    var query2 = new GetReportList(resultItem.Id);
                    var result2 = await _client.QueryAsync(query2);
                    var report = result2.Items.FirstOrDefault(x => x.Message.Contains(reportMessage));
                    if (report != null)
                    {
                        returnValue = resultItem.Id;
                        return true;
                    }

                }

                return false;
            });

            return returnValue;
        }

        public async Task<int> EnsureApplication(string name)
        {
            var query = new GetApplicationList();
            var result = await _client.QueryAsync(query);
            var app = result.FirstOrDefault(x => x.Name == name);
            if (app != null)
                return app.Id;

            var cmd = new CreateApplication(name, TypeOfApplication.DesktopApplication)
            {
                ApplicationKey = Guid.NewGuid().ToString("N"),
                UserId = 1
            };
            await _client.SendAsync(cmd);

            var query2 = new GetApplicationIdByKey(cmd.ApplicationKey);
            var retriesLeft = 3;
            while (retriesLeft-- > 0)
            {
                var app2 = await _client.QueryAsync(query2);
                if (app2 != null)
                    return app2.Id;

                await Task.Delay(500);
            }

            throw new TestFailedException("Could not create application.");
        }
    }
}
