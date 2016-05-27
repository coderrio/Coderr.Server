using System;
using System.Threading.Tasks;
using DotNetCqs;
using OneTrueError.Api.Core.Incidents;
using OneTrueError.Api.Core.Messaging;
using OneTrueError.Api.Core.Messaging.Commands;
using OneTrueError.Api.Core.Reports;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Core.Notifications.Tasks
{
    /// <summary>
    ///     Send incident emal
    /// </summary>
    public class SendIncidentEmail
    {
        private readonly ICommandBus _commandBus;

        /// <summary>
        ///     Creates a new instance of <see cref="SendIncidentEmail" />.
        /// </summary>
        /// <param name="commandBus">To send the email</param>
        /// <exception cref="ArgumentNullException">commandBus</exception>
        public SendIncidentEmail(ICommandBus commandBus)
        {
            if (commandBus == null) throw new ArgumentNullException("commandBus");
            _commandBus = commandBus;
        }

        /// <summary>
        ///     Send
        /// </summary>
        /// <param name="idOrEmailAddress">Account id or email address</param>
        /// <param name="incident">Incident that the report belongs to</param>
        /// <param name="report">Report being processed.</param>
        /// <returns>task</returns>
        /// <exception cref="ArgumentNullException">idOrEmailAddress; incident; report</exception>
        public async Task SendAsync(string idOrEmailAddress, IncidentSummaryDTO incident, ReportDTO report)
        {
            if (idOrEmailAddress == null) throw new ArgumentNullException("idOrEmailAddress");
            if (incident == null) throw new ArgumentNullException("incident");
            if (report == null) throw new ArgumentNullException("report");

            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();

            var shortName = incident.Name.Length > 20
                ? incident.Name.Substring(0, 20) + "..."
                : incident.Name;

            //TODO: Add more information
            var msg = new EmailMessage(idOrEmailAddress);
            if (incident.IsReOpened)
            {
                msg.Subject = "ReOpened: " + shortName;
                msg.TextBody = string.Format(@"Incident url: {0}/incident/{1}
Report url: {0}/incident/{1}/report/{2}
Exception: {3}

{4}
", config.BaseUrl, incident.Id, report.ReportId, report.Exception.FullName, report.Exception.StackTrace);
            }
            else if (incident.ReportCount == 1)
            {
                msg.Subject = "New: " + shortName;
                msg.TextBody = string.Format(@"Incident url: {0}/incident/{1}
Exception: {2}

{3}", config.BaseUrl, incident.Id, report.Exception.FullName, report.Exception.StackTrace);
            }
            else
            {
                msg.Subject = "Updated: " + shortName;
                msg.TextBody = string.Format(@"Incident url: {0}/incident/{1}
Report url: {0}/incident/{1}/report/{2}
Exception: {3}

{4}
", config.BaseUrl, incident.Id, report.ReportId, report.Exception.FullName, report.Exception.StackTrace);
            }

            var emailCmd = new SendEmail(msg);
            await _commandBus.ExecuteAsync(emailCmd);
        }
    }
}