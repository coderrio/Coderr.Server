using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Feedback.Commands;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Feedback.Handlers
{
    /// <summary>
    ///     Responsible of separating the feedback from the incident when it's uploaded as context data.
    /// </summary>
    public class StoreFeedbackFromNewReports : IMessageHandler<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(StoreFeedbackFromNewReports));

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            try
            {
                var userInfo = e.Report.ContextCollections.FirstOrDefault(x => x.Name == "UserSuppliedInformation");
                if (userInfo == null)
                    return;

                userInfo.Properties.TryGetValue("Description", out var description);
                userInfo.Properties.TryGetValue("Email", out var email);
                _logger.Debug($"queueing feedback attached to report {e.Report.ReportId}: {email} {description}");
                var cmd = new SubmitFeedback(e.Report.ReportId, e.Report.RemoteAddress ?? "")
                {
                    Feedback = description,
                    Email = email
                };

                await context.SendAsync(cmd);
            }
            catch (Exception exception)
            {
                _logger.Error("Failed for " + e.Report.ReportId, exception);
            }
        }
    }
}