using System.Threading.Tasks;
using Coderr.Server.Domain.Core.Feedback;
using Coderr.Server.ReportAnalyzer.Abstractions.Feedback;
using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.Feedback.Handlers
{
    /// <summary>
    ///     Responsible of attaching feedback to incidents when the feedback was uploaded before the actual incident.
    /// </summary>
    internal class AttachFeedbackToIncident : IMessageHandler<ReportAddedToIncident>
    {
        private readonly IFeedbackRepository _repository;

        public AttachFeedbackToIncident(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            var feedback = await _repository.FindPendingAsync(e.Report.ReportId);
            if (feedback == null)
                return;

            feedback.AssignToReport(e.Report.Id, e.Incident.Id, e.Incident.ApplicationId);

            var evt = new FeedbackAttachedToIncident
            {
                IncidentId = e.Incident.Id,
                Message = feedback.Description,
                UserEmailAddress = feedback.EmailAddress
            };
            await context.SendAsync(evt);
            await _repository.UpdateAsync(feedback);
        }
    }
}