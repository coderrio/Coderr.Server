using System.Threading.Tasks;
using Coderr.Server.Api.Core.Users.Commands;
using DotNetCqs;

namespace Coderr.Server.App.Core.Notifications.Commands
{
    internal class DeleteBrowserSubscriptionHandler : IMessageHandler<DeleteBrowserSubscription>
    {
        private readonly INotificationsRepository _repository;

        public DeleteBrowserSubscriptionHandler(INotificationsRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, DeleteBrowserSubscription message)
        {
            await _repository.DeleteBrowserSubscription(message.UserId, message.Endpoint);
        }
    }
}