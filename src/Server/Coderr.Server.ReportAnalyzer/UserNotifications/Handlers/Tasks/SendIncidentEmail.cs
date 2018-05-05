using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.Abstractions.ErrorReports;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.UserNotifications.Handlers.Tasks
{
    /// <summary>
    ///     Send incident email
    /// </summary>
    public class SendIncidentEmail
    {
        private readonly BaseConfiguration _baseConfiguration;

        public SendIncidentEmail(BaseConfiguration baseConfiguration)
        {
            _baseConfiguration = baseConfiguration;
        }

        /// <summary>
        ///     Send
        /// </summary>
        /// <param name="idOrEmailAddress">Account id or email address</param>
        /// <param name="incident">Incident that the report belongs to</param>
        /// <param name="report">Report being processed.</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">idOrEmailAddress; incident; report</exception>
        public async Task SendAsync(IMessageContext context, string idOrEmailAddress, IncidentSummaryDTO incident,
            ReportDTO report)
        {
            if (idOrEmailAddress == null) throw new ArgumentNullException("idOrEmailAddress");
            if (incident == null) throw new ArgumentNullException("incident");
            if (report == null) throw new ArgumentNullException("report");

            var shortName = incident.Name.Length > 40
                ? incident.Name.Substring(0, 40) + "..."
                : incident.Name;

            var pos = shortName.IndexOfAny(new[] {'\r', '\n'});
            if (pos != -1)
            {
                shortName = shortName.Substring(0, pos) + "[...]";
            }
            

            var baseUrl = _baseConfiguration.BaseUrl.ToString().TrimEnd('/');
            var incidentUrl =
                $"{baseUrl}/discover/incidents/{report.ApplicationId}/incident/{report.IncidentId}/";

            //TODO: Add more information
            var msg = new EmailMessage(idOrEmailAddress);
            if (incident.IsReOpened)
            {
                msg.Subject = "ReOpened: " + shortName;
                msg.TextBody = $@"{incident.Name}
{report.Exception.FullName}
{report.Exception.StackTrace}

{incidentUrl}";
            }
            else if (incident.ReportCount == 1)
            {
                msg.Subject = "New: " + shortName;
                msg.TextBody = $@"{incident.Name}
{report.Exception.FullName}
{report.Exception.StackTrace}

{incidentUrl}";
            }
            else
            {
                msg.Subject = "Updated: " + shortName;
                msg.TextBody = $@"{incident.Name}
{report.Exception.FullName}
{report.Exception.StackTrace}

{incidentUrl}";
            }

            var emailCmd = new SendEmail(msg);
            await context.SendAsync(emailCmd);
        }
    }
}