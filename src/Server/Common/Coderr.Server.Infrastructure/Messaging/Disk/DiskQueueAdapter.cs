using Coderr.Server.Infrastructure.Messaging.Disk.DotNetCqs;
using Coderr.Server.Infrastructure.Messaging.Disk.Queue;
using DotNetCqs;
using DotNetCqs.Queues;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    public class DiskQueueAdapter : IMessageQueue
    {
        private readonly DiskQueue<Message> _queue;

        public DiskQueueAdapter(string name, DiskQueue<Message> queue)
        {
            _queue = queue;
            Name = name;
        }

        public IMessageQueueSession BeginSession()
        {
            return new DiskQueueSession(_queue);
        }

        public string Name { get; }
    }
}