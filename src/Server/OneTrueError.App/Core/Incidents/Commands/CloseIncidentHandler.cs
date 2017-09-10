using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Commands;
using OneTrueError.Api.Core.Messaging;
using OneTrueError.Api.Core.Messaging.Commands;
using OneTrueError.App.Core.Feedback;

namespace OneTrueError.App.Core.Incidents.Commands
{
    /// <summary>
    ///     Handler of <see cref="CloseIncident" />.
    /// </summary>
    [Component]
    public class CloseIncidentHandler : ICommandHandler<CloseIncident>
    {
        private readonly ICommandBus _commandBus;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IIncidentRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="CloseIncidentHandler" />.
        /// </summary>
        /// <param name="repository">To be able to load and update incident</param>
        /// <param name="feedbackRepository">To be able to see if someone is waiting on update notifications.</param>
        /// <param name="commandBus">To send notification emails.</param>
        public CloseIncidentHandler(IIncidentRepository repository, IFeedbackRepository feedbackRepository,
            ICommandBus commandBus)
        {
            _repository = repository;
            _feedbackRepository = feedbackRepository;
            _commandBus = commandBus;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task ExecuteAsync(CloseIncident command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var incident = await _repository.GetAsync(command.IncidentId);
            incident.Solve(command.UserId, command.Solution);
            if (command.ShareSolution)
                incident.ShareSolution();

            if (command.CanSendNotification && !string.IsNullOrEmpty(command.NotificationTitle) &&
                !string.IsNullOrEmpty(command.NotificationText))
            {
                var emails = await _feedbackRepository.GetEmailAddressesAsync(command.IncidentId);
                var emailMessage = new EmailMessage(emails)
                {
                    Subject = command.NotificationTitle,
                    TextBody = command.NotificationText
                };
                var sendMessage = new SendEmail(emailMessage);
                await _commandBus.ExecuteAsync(sendMessage);
            }

            //var reports = _reportsRepository.GetAll(incident.Reports.Select(x => x.ReportId).ToArray());
            //foreach (var report in reports)
            //{
            //    report.Solve(command.Solution);
            //    _reportsRepository.Update(report);
            //}

            await _repository.UpdateAsync(incident);
        }
    }
}