using System;
using System.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Queueing.Ado;
#if NET452
using codeRR.Server.Infrastructure.Queueing.Msmq;
#endif
using Griffin.Container;

namespace codeRR.Server.Infrastructure.Queueing
{
    /// <summary>
    ///     Purpose of this class is to abstract away the queue creation and coupling to specific implementations (and their
    ///     life times).
    /// </summary>
    [Component(Lifetime = Lifetime.Singleton)]
    public class QueueProvider : IMessageQueueProvider
    {
        private IMessageQueue _eventQueue;
        private IMessageQueue _feedbackQueue;
        private IMessageQueue _reportQueue;
        private IConnectionFactory _connectionFactory;

        public QueueProvider(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IMessageQueue Open(string queueName)
        {
            var config = ConfigurationStore.Instance.Load<MessageQueueSettings>()
                         ?? new MessageQueueSettings(); //isn't added by the installation guide.

            switch (queueName)
            {
                case "ReportQueue":
                    if (_reportQueue != null)
                        return _reportQueue;
                    _reportQueue = CreateQueue("Reports", config);
                    return _reportQueue;
                case "FeedbackQueue":
                    if (_feedbackQueue != null)
                        return _feedbackQueue;
                    _feedbackQueue = CreateQueue("Feedback", config);
                    return _feedbackQueue;
                case "EventQueue":
                    if (_eventQueue != null)
                        return _eventQueue;
                    _eventQueue = CreateQueue("Events", config);
                    return _eventQueue;
                default:
                    throw new NotSupportedException("Queue is not found: " + queueName);
            }
        }

        protected virtual IMessageQueue CreateQueue(string queueName, MessageQueueSettings config)
        {
            if (!config.UseSql)
            {
#if NET452
                return new MsmqMessageQueue(config.ReportQueue, config.ReportAuthentication,
                    config.ReportTransactions);
#else
                throw new NotSupportedException("MSMQ is currently not supported for .NET Standard.");
#endif
            }
                


            return new AdoNetMessageQueue(queueName, _connectionFactory);
        }
    }
}