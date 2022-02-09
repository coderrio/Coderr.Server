using System;
using System.Collections.Generic;
using System.Linq;
using DotNetCqs.Queues;

namespace Coderr.Server.Infrastructure.Messaging.Tests
{
    public class TestQueue : IMessageQueue
    {
        public List<TestQueueSession> Sessions = new List<TestQueueSession>();
        public Queue<TestQueueSession> SessionsToReturn = new Queue<TestQueueSession>();

        public TestQueue(string queueName)
        {
            Name = queueName;
        }

        public bool HasBegun { get; set; }

        public IMessageQueueSession BeginSession()
        {
            var session = SessionsToReturn.Count > 0 ? SessionsToReturn.Dequeue() : new TestQueueSession();

            Sessions.Add(session);
            return session;
        }

        public TestMessageWrapper Dequeue()
        {
            if (Sessions.Count == 0)
                return null;

            var session = Sessions.First(x => x.EnqueuedCount > 0);
            var msg = session.Enqueued.Dequeue();
            return msg;
        }

        public string Name { get; }
    }
}