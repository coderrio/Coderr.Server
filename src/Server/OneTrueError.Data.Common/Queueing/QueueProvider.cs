using System;
using System.Configuration;
using Griffin.Container;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Infrastructure.Queueing.Ado;
using OneTrueError.Infrastructure.Queueing.Msmq;

namespace OneTrueError.Infrastructure.Queueing
{
    /// <summary>
    /// Purpose of this class is to abstract away the queue creation and coupling to specific implementations (and their life times).
    /// </summary>
    [Component(Lifetime = Lifetime.Singleton)]
    public class QueueProvider : IMessageQueueProvider
    {
        private IMessageQueue _eventQueue;
        private IMessageQueue _feedbackQueue;
        private IMessageQueue _reportQueue;

        public IMessageQueue Open(string queueName)
        {
            var config = ConfigurationStore.Instance.Load<MessageQueueSettings>()
                ?? new MessageQueueSettings(); //isn't added by the installation guide.
            var conString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            var provider = ConfigurationManager.ConnectionStrings["Db"].ProviderName;
            switch (queueName)
            {
                case "ReportQueue":
                    if (_reportQueue != null)
                        return _reportQueue;
                    _reportQueue = config.UseSql
                        ? (IMessageQueue) new AdoNetMessageQueue("Reports", provider, conString)
                        : new MsmqMessageQueue(config.ReportQueue, config.ReportAuthentication,
                            config.ReportTransactions);
                    return _reportQueue;
                case "FeedbackQueue":
                    if (_feedbackQueue != null)
                        return _feedbackQueue;
                    _feedbackQueue = config.UseSql
                        ? (IMessageQueue) new AdoNetMessageQueue("Feedback", provider, conString)
                        : new MsmqMessageQueue(config.FeedbackQueue, config.FeedbackAuthentication,
                            config.FeedbackTransactions);
                    return _feedbackQueue;
                case "EventQueue":
                    if (_eventQueue != null)
                        return _eventQueue;
                    _eventQueue = config.UseSql
                        ? (IMessageQueue) new AdoNetMessageQueue("Events", provider, conString)
                        : new MsmqMessageQueue(config.EventQueue, config.EventAuthentication,
                            config.EventTransactions);
                    return _eventQueue;
                default:
                    throw new NotSupportedException("Queue is not found: " + queueName);
            }
        }
    }
}