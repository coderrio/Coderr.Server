using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure.Messaging.Disk.Queue;
using DotNetCqs;
using DotNetCqs.Queues;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk.DotNetCqs
{
    public class DiskQueueSession : IMessageQueueSession
    {
        private readonly DiskQueue<Message> _queue;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DiskQueueSession));

        public DiskQueueSession(DiskQueue<Message> queue)
        {
            _queue = queue;
        }

        public void Dispose()
        {
        }

        public async Task<Message> Dequeue(TimeSpan suggestedWaitPeriod)
        {
            return await _queue.DequeueAsync();
        }

        public async Task<DequeuedMessage> DequeueWithCredentials(TimeSpan suggestedWaitPeriod)
        {
            var msg = await _queue.DequeueAsync();
            if (msg == null) return null;

            var principal = CreatePrincipal(msg);
            return new DequeuedMessage(principal, msg);
        }

        public async Task EnqueueAsync(ClaimsPrincipal principal, IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                foreach (var claim in principal.Claims) message.Properties[$"X-Claim-{claim.Type}"] = claim.Value;

                message.Properties["X-Principal-AuthenticationType"] = principal.Identity.AuthenticationType;
                message.Properties["Body-Type"] = message.Body.GetType().FullName;
                await _queue.EnqueueAsync(message);
            }

            await _queue.FlushAsync();
        }

        public async Task EnqueueAsync(IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                message.Properties["Body-Type"] = message.Body.GetType().FullName;
                await _queue.EnqueueAsync(message);
            }

            await _queue.FlushAsync();
        }

        public async Task EnqueueAsync(ClaimsPrincipal principal, Message message)
        {
            foreach (var claim in principal.Claims) message.Properties[$"X-Claim-{claim.Type}"] = claim.Value;

            message.Properties["X-Principal-AuthenticationType"] = principal.Identity.AuthenticationType;
            message.Properties["Body-Type"] = message.Body.GetType().FullName;
            await _queue.EnqueueAsync(message);
            await _queue.FlushAsync();
        }

        public async Task EnqueueAsync(Message message)
        {
            message.Properties["Body-Type"] = message.Body.GetType().FullName;
            await _queue.EnqueueAsync(message);
            await _queue.FlushAsync();
        }

        public Task SaveChanges()
        {
            return Task.CompletedTask;
        }

        private ClaimsPrincipal CreatePrincipal(Message msg)
        {
            if (msg.Properties == null)
            {
                _logger.Debug("Null principal" + msg.Body);
                return null;
            }

            if (!msg.Properties.TryGetValue("X-Principal-AuthenticationType", out var authType))
                return null;

            var claims = new List<Claim>();
            foreach (var property in msg.Properties)
            {
                if (!property.Key.StartsWith("X-Claim-"))
                    continue;

                var name = property.Key.Substring(8);
                var value = property.Value;
                claims.Add(new Claim(name, value));
            }

            return new ClaimsPrincipal(new ClaimsIdentity(claims, authType));
        }
    }
}