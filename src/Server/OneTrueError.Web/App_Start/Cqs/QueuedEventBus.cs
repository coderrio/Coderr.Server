using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.ApplicationServices;
using log4net;
using Newtonsoft.Json;
using OneTrueError.Infrastructure.Queueing;

namespace OneTrueError.Web.Cqs
{
    public class QueuedEventBus : ApplicationServiceThread, IEventBus, IDisposable
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(QueuedEventBus));
        private readonly IMessageQueue _queue;
        private readonly IEventBus _writeBus;

        public QueuedEventBus(IEventBus writeBus, IMessageQueueProvider queueProvider)
        {
            _queue = queueProvider.Open("EventQueue");
            _writeBus = writeBus;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public Task PublishAsync<TApplicationEvent>(TApplicationEvent e)
            where TApplicationEvent : ApplicationEvent
        {
            try
            {
                _logger.Debug("Enqueueing: " + e.GetType().Name + " " + JsonConvert.SerializeObject(e));
                _queue.Write(0, e);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to publish " + JsonConvert.SerializeObject(e), ex);
                throw;
            }

            return Task.FromResult<object>(null);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            //TODO: Dispose queue
        }

        protected override void Run(WaitHandle shutdownHandle)
        {
            _logger.Debug("Starting up event queue...");
            while (!shutdownHandle.WaitOne(0))
            {
                object msg = null;
                try
                {
                    msg = _queue.Receive();
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to receive.", ex);
                    if (shutdownHandle.WaitOne(10000))
                        break;
                    continue;
                }

                if (msg == null)
                {
                    if (shutdownHandle.WaitOne(1000))
                        break;
                    continue;
                }

                try
                {
                    ExecuteMessage(msg);
                }
                catch (Exception ex)
                {
                    _logger.Error("Processing '" + JsonConvert.SerializeObject(msg) + "' failed.", ex);
                }
            }
        }

        private void ExecuteMessage(object message)
        {
            _logger.Debug("PUBLISHING " + message.GetType().Name + " " + JsonConvert.SerializeObject(message));
            var method = typeof(IEventBus).GetMethod("PublishAsync");
            var mi = method.MakeGenericMethod(message.GetType());
            var task = (Task) mi.Invoke(_writeBus, new[] {message});
            task.Wait();
            _logger.Debug("Done PUBLISHING " + message.GetType().Name);
        }
    }
}