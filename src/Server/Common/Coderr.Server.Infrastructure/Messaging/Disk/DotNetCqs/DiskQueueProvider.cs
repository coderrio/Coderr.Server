using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure.Messaging.Disk.Queue;
using DotNetCqs;
using DotNetCqs.Queues;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging.Disk.DotNetCqs
{
    public class DiskQueueProvider : IMessageQueueProvider, IDisposable
    {
        private readonly string _queueDirectory;

        private readonly Dictionary<string, DiskQueue<Message>> _queues =
            new Dictionary<string, DiskQueue<Message>>();

        private QueueLockFile _lockFile;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DiskQueueProvider));

        public DiskQueueProvider(string queueDirectory)
        {
            _queueDirectory = queueDirectory;
        }

        public Func<Task<bool>> ShutdownRequested { get; set; }

        public void Dispose()
        {
            if (_queues.Any()) Shutdown();
        }

        public IMessageQueue Open(string queueName)
        {
            
            if (_queues.TryGetValue(queueName, out var entry)) return new DiskQueueAdapter(queueName, entry);

            if (_lockFile == null)
            {
                _lockFile = new QueueLockFile(_queueDirectory, queueName) {CloseQueueRequested = TriggerQueueEvent};
                _lockFile.CreateLockFile(TimeSpan.FromSeconds(30)).GetAwaiter().GetResult();
            }

            var queue = new DiskQueue<Message>(_queueDirectory, queueName);
            _queues.Add(queueName, queue);
            queue.OpenAsync(TimeSpan.FromSeconds(60)).GetAwaiter().GetResult();

            return new DiskQueueAdapter(queueName, queue);
        }

        public void Shutdown()
        {
            foreach (var queue in _queues)
            {
                try
                {
                    queue.Value.CloseAsync().GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    _logger.Error("Failed to close " + queue.Key, ex);
                }
            }

            foreach (var queue in _queues) queue.Value.Dispose();

            _queues.Clear();
            _lockFile.DeleteLockFile();
        }

        private async Task<bool> TriggerQueueEvent()
        {
            if (ShutdownRequested == null)
                return false;

            return await ShutdownRequested();
        }
    }
}