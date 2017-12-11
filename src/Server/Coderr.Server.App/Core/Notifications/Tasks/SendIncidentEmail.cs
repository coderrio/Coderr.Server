using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents;
using codeRR.Server.Api.Core.Messaging;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;

namespace codeRR.Server.App.Core.Notifications.Tasks
{
    /// <summary>
    ///     Send incident email
    /// </summary>
    public class SendIncidentEmail
    {
        private ConfigurationStore _configStore;

        public SendIncidentEmail(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        /// <summary>
        ///     Send
        /// </summary>
        /// <param name="idOrEmailAddress">Account id or email address</param>
        /// <param name="incident">Incident that the report belongs to</param>
        /// <param name="report">Report being processed.</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">idOrEmailAddress; incident; report</exception>
        public async Task SendAsync(IMessageContext context, string idOrEmailAddress, IncidentSummaryDTO incident, ReportDTO report)
        {
            if (idOrEmailAddress == null) throw new ArgumentNullException("idOrEmailAddress");
            if (incident == null) throw new ArgumentNullException("incident");
            if (report == null) throw new ArgumentNullException("report");

            var config = _configStore.Load<BaseConfiguration>();

            var shortName = incident.Name.Length > 40
                ? incident.Name.Substring(0, 40) + "..."
                : incident.Name;

            // need to be safe for subjects
            shortName = shortName.Replace("\n", ";");

            var baseUrl = string.Format("{0}/#/application/{1}/incident/{2}/",
                config.BaseUrl.ToString().TrimEnd('/'),
                report.ApplicationId,
                report.IncidentId);

            //TODO: Add more information
            var msg = new EmailMessage(idOrEmailAddress);
            if (incident.IsReOpened)
            {
                msg.Subject = "ReOpened: " + shortName;
                msg.TextBody = string.Format(@"Incident: {0}
Report url: {0}/report/{1}/
Description: {2}
Exception: {3}

{4}
", baseUrl, report.Id, incident.Name, report.Exception.FullName, report.Exception.StackTrace);
            }
            else if (incident.ReportCount == 1)
            {
                msg.Subject = "New: " + shortName;
                msg.TextBody = string.Format(@"Incident: {0}
Description: {1}
Exception: {2}

{3}", baseUrl, incident.Name, report.Exception.FullName, report.Exception.StackTrace);
            }
            else
            {
                msg.Subject = "Updated: " + shortName;
                msg.TextBody = string.Format(@"Incident: {0}
Report url: {0}/report/{1}/
Description: {2}
Exception: {3}

{4}
", baseUrl, report.Id, incident.Name, report.Exception.FullName, report.Exception.StackTrace);
            }

            var emailCmd = new SendEmail(msg);
            await context.SendAsync(emailCmd);
        }
    }
}
