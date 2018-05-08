using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Commands;
using Coderr.Server.Api.Core.Messaging;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.Domain.Core.Feedback;
using DotNetCqs;

namespace Coderr.Server.App.Core.Incidents.Commands
{
    internal class NotifySubscribersHandler : IMessageHandler<NotifySubscribers>
    {
        private readonly IFeedbackRepository _feedbackRepository;

        public NotifySubscribersHandler(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }


        public async Task HandleAsync(IMessageContext context, NotifySubscribers message)
        {
            var emails = await _feedbackRepository.GetEmailAddressesAsync(message.IncidentId);
            if (!emails.Any())
                return;

            var emailMessage = new EmailMessage(emails)
            {
                Subject = message.Title,
                TextBody = message.Body
            };
            var sendMessage = new SendEmail(emailMessage);
            await context.SendAsync(sendMessage);
        }
    }
}