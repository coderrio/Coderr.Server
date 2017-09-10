using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Feedback.Events;
using OneTrueError.Api.Core.Incidents.Events;

namespace OneTrueError.App.Core.Feedback.EventSubscribers
{
    /// <summary>
    ///     Responsible of attaching feedback to incidents when the feedback was uploaded before the actual incident.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    internal class AttachFeedbackToIncident : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly IEventBus _eventBus;
        private readonly IFeedbackRepository _repository;

        public AttachFeedbackToIncident(IFeedbackRepository repository, IEventBus eventBus)
        {
            if (eventBus == null) throw new ArgumentNullException("eventBus");
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task HandleAsync(ReportAddedToIncident e)
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
            await _eventBus.PublishAsync(evt);
            await _repository.UpdateAsync(feedback);
        }
    }
}