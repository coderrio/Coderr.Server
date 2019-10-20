using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.TestFramework;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Core.Applications;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Api.Core.Environments.Commands;
using Coderr.Server.Api.Core.Environments.Queries;
using Coderr.Server.Api.Core.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Api.Core.Reports.Queries;
using DotNetCqs;

namespace Coderr.IntegrationTests.Core.Tools
{
    public static class ReportingApiExtensions
    {
        public static async Task<bool> CheckIfReportExists(this ServerApiClient client, int applicationId,
            string genericMessage, string reportDetails)
        {
            var report = await client.GetReportAsync(applicationId, reportDetails);
            return report != null;
        }

        public static async Task<int> EnsureApplication(this ServerApiClient client, string name)
        {
            var query = new GetApplicationList();
            var result = await client.QueryAsync(query);
            var app = result.FirstOrDefault(x => x.Name == name);
            if (app != null)
                return app.Id;

            var cmd = new CreateApplication(name, TypeOfApplication.DesktopApplication)
            {
                ApplicationKey = Guid.NewGuid().ToString("N"),
                UserId = 1
            };
            await client.SendAsync(cmd);

            var query2 = new GetApplicationIdByKey(cmd.ApplicationKey);
            var retriesLeft = 3;
            while (retriesLeft-- > 0)
            {
                var app2 = await client.QueryAsync(query2);
                if (app2 != null)
                    return app2.Id;

                await Task.Delay(500);
            }

            throw new TestFailedException("Could not create application.");
        }

        public static async Task<GetIncidentResult> GetIncident(this ServerApiClient client, int applicationId,
            string incidentMessage, string reportMessage = null)
        {
            var id = await client.GetIncidentId(applicationId, incidentMessage, reportMessage);
            if (id == null)
                throw new InvalidOperationException("Failed to find our uploaded report");


            var query = new GetIncident(id.Value);
            return await client.QueryAsync(query);
        }

        public static async Task<GetReportResult> GetReport(this ServerApiClient client, int incidentId, string partOfErrorMessage)
        {
            var reportListItem = await client.GetReportAsync(incidentId, partOfErrorMessage);
            if (reportListItem == null)
                throw new InvalidOperationException("Failed to find our uploaded report");

            var query3 = new GetReport(reportListItem.Id);
            return await client.QueryAsync(query3);
        }

        public static async Task Reset(this ServerApiClient client, int applicationId, string environmentName)
        {
            var envs = new GetEnvironments();
            var result = await client.QueryAsync(envs);
            var id = result.Items.FirstOrDefault(x => x.Name == environmentName)?.Id;

            // haven't reported for that environment yet
            if (id == null)
                return;

            var cmd = new ResetEnvironment(applicationId, id.Value);
            await client.SendAsync(cmd);
        }

        private static async Task<int?> GetIncidentId(this ServerApiClient client, int applicationId,
            string incidentMessage, string reportMessage = null)
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
                var result = await client.QueryAsync(query);
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
                    var result2 = await client.QueryAsync(query2);
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
                    var result2 = await client.QueryAsync(query2);
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

        private static async Task<GetReportListResultItem> GetReportAsync(this ServerApiClient client, int incidentId, string partOfErrorMessage)
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var query2 = new GetReportList(incidentId);
                var result2 = await client.QueryAsync(query2);
                var report = result2.Items.FirstOrDefault(x => x.Message.Contains(partOfErrorMessage));
                if (report != null)
                    return report;

                await Task.Delay((3 - triesLeft) * 2000);
            }

            return null;
        }

        private static async Task Repeat(Func<Task<bool>> logic)
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                if (await logic())
                    break;

                await Task.Delay((3 - triesLeft) * 2000);
            }
        }

        private static async Task<TResult> TryQuery<TQuery, TResult>(this ServerApiClient client, TQuery query)
            where TQuery : Query<TResult>
        {
            var triesLeft = 3;
            while (triesLeft-- > 0)
            {
                var result = await client.QueryAsync(query);
                if (result != null)
                {
                    var collection = (IList)result.GetType().GetProperty("Items")?.GetValue(result);
                    if (collection != null)
                        if (collection.Count > 0)
                            return result;

                    var prop = result.GetType().GetProperties()
                        .FirstOrDefault(x => x.PropertyType.GetProperty("Count") != null);
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
    }
}