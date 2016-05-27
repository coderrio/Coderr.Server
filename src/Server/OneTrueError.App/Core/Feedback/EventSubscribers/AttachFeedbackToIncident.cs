using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Events;

namespace OneTrueError.App.Core.Feedback.EventSubscribers
{
    /// <summary>
    ///     Responsible of attaching feedback to incidents when the feedback was uploaded before the actual incident.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    internal class AttachFeedbackToIncident : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly IFeedbackRepository _repository;

        public AttachFeedbackToIncident(IFeedbackRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(ReportAddedToIncident e)
        {
            var feedback = await _repository.FindPendingAsync(e.Report.ReportId);
            if (feedback == null)
                return;

            feedback.AssignToReport(e.Report.Id, e.Incident.Id, e.Incident.ApplicationId);
            await _repository.UpdateAsync(feedback);
        }
    }
}