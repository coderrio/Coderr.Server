using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Feedback.Commands;
using codeRR.Server.Api.Core.Incidents.Events;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Core.Feedback.EventSubscribers
{
    /// <summary>
    ///     Responsible of separating the feedback from the incident when it's uploaded as context data.
    /// </summary>
    [Component(RegisterAsSelf = true)]
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
                _logger.Debug("storing feedback for report " + e.Report.ReportId);
                var userInfo = Enumerable.FirstOrDefault(e.Report.ContextCollections, x => x.Name == "UserSuppliedInformation");
                if (userInfo == null)
                    return;

                string description;
                string email;
                userInfo.Properties.TryGetValue("Description", out description);
                userInfo.Properties.TryGetValue("Email", out email);

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