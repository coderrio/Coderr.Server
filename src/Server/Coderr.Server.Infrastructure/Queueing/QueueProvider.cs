using System;
using System.Configuration;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Queueing.Ado;
using codeRR.Server.Infrastructure.Queueing.Msmq;
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
        /// <summary>
        ///     To allow extensions (not the most fantastic way to do it, more of a hack). Got to be able to update the container
        ///     service registry.
        /// </summary>
        public static Func<string, MessageQueueSettings, IMessageQueue> CreateQueueHandler;

        private IMessageQueue _eventQueue;
        private IMessageQueue _feedbackQueue;
        private IMessageQueue _reportQueue;


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
            if (CreateQueueHandler != null)
                return CreateQueueHandler(queueName, config);


            if (!config.UseSql)
                return new MsmqMessageQueue(config.ReportQueue, config.ReportAuthentication,
                    config.ReportTransactions);


            var conStr = ConfigurationManager.ConnectionStrings["Queue"]
                         ?? ConfigurationManager.ConnectionStrings["Db"];
            var conString = conStr.ConnectionString;
            var provider = conStr.ProviderName;

            return new AdoNetMessageQueue(queueName, provider, conString);
        }
    }
}