using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using codeRR.Api.Core.Feedback.Commands;
using codeRR.Api.Core.Incidents.Events;

namespace codeRR.App.Core.Feedback.EventSubscribers
{
    /// <summary>
    ///     Responsible of separating the feedback from the incident when it's uploaded as context data.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class StoreFeedbackFromNewReports : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly ICommandBus _commandBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(StoreFeedbackFromNewReports));

        /// <summary>
        ///     Creates a new instance of <see cref="StoreFeedbackFromNewReports" />.
        /// </summary>
        /// <param name="commandBus">to send the <see cref="SubmitFeedback" /> command</param>
        /// <exception cref="ArgumentNullException">commandBus</exception>
        public StoreFeedbackFromNewReports(ICommandBus commandBus)
        {
            if (commandBus == null) throw new ArgumentNullException("commandBus");
            _commandBus = commandBus;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(ReportAddedToIncident e)
        {
            try
            {
                _logger.Debug("storing feedback for report " + e.Report.ReportId);
                var userInfo = e.Report.ContextCollections.FirstOrDefault(x => x.Name == "UserSuppliedInformation");
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

                await _commandBus.ExecuteAsync(cmd);
            }
            catch (Exception exception)
            {
                _logger.Error("Failed for " + e.Report.ReportId, exception);
            }
        }
    }
}