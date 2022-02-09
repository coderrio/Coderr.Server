using Coderr.Server.Infrastructure.Messaging;
using DotNetCqs.Queues;
using DotNetCqs.Queues.Azure.ServiceBus;
using log4net;

namespace Coderr.Server.Web.Boot.Cqs
{
    public class AzureQueueProvider : IMessageQueueProvider
    {
        private readonly string _connectionString;
        private ILog _logger = LogManager.GetLogger(typeof(AzureQueueProvider));


        public AzureQueueProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IMessageQueue Open(string queueName)
        {
            _logger.Info("Opening queue " + queueName);
            return new AzureMessageQueue(_connectionString, queueName) { MessageSerializer = new MessagingSerializer() };
        }
    }
}
