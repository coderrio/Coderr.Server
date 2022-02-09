using System.Collections.Generic;
using DotNetCqs.Queues;

namespace Coderr.Server.Infrastructure.Messaging.Tests
{
    public class TestQueueProvider : IMessageQueueProvider
    {
        private Dictionary<string, TestQueue> _queues = new Dictionary<string, TestQueue>();

        public IMessageQueue Open(string queueName)
        {
            if (!_queues.TryGetValue(queueName, out var queue))
            {
                queue = new TestQueue(queueName);
                _queues[queueName] = queue;
            }


            return queue;
        }

        public TestQueue this[string name] => _queues[name];
    }
}