using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCqs;
using DotNetCqs.Queues;

namespace Coderr.Server.Infrastructure.Messaging.Tests
{
    public class TestQueueSession : IMessageQueueSession
    {
        private readonly Queue<TestMessageWrapper> _enqueued = new Queue<TestMessageWrapper>();
        private readonly Queue<TestMessageWrapper> _dequeued = new Queue<TestMessageWrapper>();

        public bool IsDisposed { get; private set; }

        public bool IsSaved { get; private set; }
        public int EnqueuedCount => _enqueued.Count;

        public Queue<TestMessageWrapper> Enqueued => _enqueued;

        public void Dispose()
        {
            IsDisposed = true;
        }

        public Task<Message> Dequeue(TimeSpan suggestedWaitPeriod)
        {
            return Task.FromResult(_dequeued.Dequeue()?.Message);
        }

        public Task<DequeuedMessage> DequeueWithCredentials(TimeSpan suggestedWaitPeriod)
        {
            var message = _dequeued.Dequeue();
            return message == null
                ? null
                : Task.FromResult(new DequeuedMessage(message.Principal, message.Message));
        }

        public Task EnqueueAsync(ClaimsPrincipal principal, IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                _enqueued.Enqueue(new TestMessageWrapper(principal, message));
            }

            return Task.CompletedTask;
        }

        public Task EnqueueAsync(IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages) _enqueued.Enqueue(new TestMessageWrapper(null, message));

            return Task.CompletedTask;
        }

        public Task EnqueueAsync(ClaimsPrincipal principal, Message message)
        {
            _enqueued.Enqueue(new TestMessageWrapper(principal, message));
            return Task.CompletedTask;
        }

        public Task EnqueueAsync(Message message)
        {
            _enqueued.Enqueue(new TestMessageWrapper(null, message));
            return Task.CompletedTask;
        }

        public Task SaveChanges()
        {
            IsSaved = true;
            return Task.CompletedTask;
        }
    }
}