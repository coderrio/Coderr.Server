using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCqs;
using DotNetCqs.Queues;

namespace Coderr.Server.WebSite.Hubs
{
    public class HubSession : IMessageQueueSession
    {
        private readonly WebSocketHub _webSocketHub;
        private List<Message> _messages = new List<Message>();
        private ClaimsPrincipal _principal;


        public HubSession(WebSocketHub webSocketHub)
        {
            _webSocketHub = webSocketHub;
        }

        public void Dispose()
        {

        }

        public Task<Message> Dequeue(TimeSpan suggestedWaitPeriod)
        {
            throw new NotSupportedException();
        }

        public Task<DequeuedMessage> DequeueWithCredentials(TimeSpan suggestedWaitPeriod)
        {
            throw new NotSupportedException();
        }

        public Task EnqueueAsync(ClaimsPrincipal principal, IReadOnlyCollection<Message> messages)
        {
            AssignPrincipal(principal);
            foreach (var message in messages)
            {
                _messages.Add(message);
            }

            return Task.CompletedTask;
        }

        private void AssignPrincipal(ClaimsPrincipal principal)
        {
            if (_principal != null && _principal.Identity.Name != principal.Identity.Name)
            {
                throw new InvalidOperationException("Expected same principal");
            }

            _principal = principal;
        }

        public Task EnqueueAsync(IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                _messages.Add(message);
            }
            return Task.CompletedTask;

        }

        public Task EnqueueAsync(ClaimsPrincipal principal, Message message)
        {
            AssignPrincipal(principal);

            _messages.Add(message);
            return Task.CompletedTask;
        }

        public Task EnqueueAsync(Message message)
        {
            _messages.Add(message);
            return Task.CompletedTask;

        }

        public async Task SaveChanges()
        {
            await _webSocketHub.Send(_principal, _messages);
        }
    }
}