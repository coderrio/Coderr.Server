using System;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Users.Commands;
using Coderr.Server.Domain.Modules.UserNotifications;
using DotNetCqs;

namespace Coderr.Server.App.Core.Notifications.Commands
{
    internal class StoreBrowserSubscriptionHandler : IMessageHandler<StoreBrowserSubscription>
    {
        private readonly INotificationsRepository _notificationsRepository;

        public StoreBrowserSubscriptionHandler(INotificationsRepository notificationsRepository)
        {
            _notificationsRepository = notificationsRepository;
        }

        public async Task HandleAsync(IMessageContext context, StoreBrowserSubscription message)
        {
            var subscription = new BrowserSubscription
            {
                AccountId = context.Principal.GetAccountId(),
                AuthenticationSecret = message.AuthenticationSecret,
                Endpoint = message.Endpoint,
                PublicKey = message.PublicKey,
                ExpiresAtUtc = message.ExpirationTime == null
                    ? (DateTime?)null
                    : new DateTime(1970, 1, 1).AddMilliseconds(message.ExpirationTime.Value)
            };
            await _notificationsRepository.Save(subscription);
        }
    }
}