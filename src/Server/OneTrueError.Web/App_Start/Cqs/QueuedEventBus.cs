using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.ApplicationServices;
using Griffin.Container;
using log4net;
using Newtonsoft.Json;
using OneTrueError.Infrastructure.Queueing;

namespace OneTrueError.Web.Cqs
{
    public class QueuedEventBus : ApplicationServiceThread, IEventBus, IDisposable
    {
        private readonly IEventBus _writeBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof (QueuedEventBus));
        private readonly IMessageQueue _queue;

        private int _retryCounter;

        public QueuedEventBus(IEventBus writeBus, IMessageQueueProvider queueProvider)
        {
            _queue = queueProvider.Open("EventQueue");
            _writeBus = writeBus;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            //TODO: Dispose queue
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task PublishAsync<TApplicationEvent>(TApplicationEvent e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            where TApplicationEvent : ApplicationEvent
        {
            try
            {
                _logger.Debug("Publishing: " + JsonConvert.SerializeObject(e));
                _queue.Write(0, e);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to publish " + JsonConvert.SerializeObject(e), ex);
                throw;
            }
        }

        protected override void Run(WaitHandle shutdownHandle)
        {
            while (true)
            {
                object msg = null;
                try
                {
                    msg = _queue.Receive();
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to receive.", ex);
                    Thread.Sleep(10000);
                    continue;
                }

                if (msg == null)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                while (_retryCounter < 3)
                {
                    try
                    {
                        ExecuteMessage(msg);
                        break;
                    }
                    catch (Exception ex)
                    {
                        _retryCounter++;
                        if (_retryCounter < 3)
                            _logger.Warn("Processing '" + JsonConvert.SerializeObject(msg) + "' failed, retrying..");
                        else
                            _logger.Error("Processing '" + JsonConvert.SerializeObject(msg) + "' failed.", ex);
                    }
                }
            }
        }

        private void ExecuteMessage(object message)
        {
            var method = typeof (IEventBus).GetMethod("PublishAsync");
            var mi = method.MakeGenericMethod(message.GetType());
            var task = (Task) mi.Invoke(_writeBus, new[] {message});
            task.Wait();
        }
    }
}