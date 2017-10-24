using codeRR.Server.Infrastructure;
using codeRR.Server.Infrastructure.MessageBus;
using DotNetCqs.Queues;
using DotNetCqs.Queues.AdoNet;
using Griffin.Container;

namespace codeRR.Server.Web.Cqs
{
    [Component(Lifetime = Lifetime.Singleton)]
    public class MessageQueueProvider : IMessageQueueProvider
    {
        private readonly IConnectionFactory _connectionFactory;

        public MessageQueueProvider(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IMessageQueue Open(string queueName)
        {
            return new AdoNetMessageQueue(queueName,
                () => _connectionFactory.TryOpen("Queue") ?? _connectionFactory.Open(), new JsonMessageQueueSerializer())
            {
                TableName = "MessageQueue"
            };
        }
    }
}