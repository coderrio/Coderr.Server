using System.Collections.Generic;
using System.Linq;

namespace Coderr.Tests.Events
{
    public class AggregateEventReceiver : IEventReceiver
    {
        private readonly List<IEventReceiver> _receivers;

        public AggregateEventReceiver(IEnumerable<IEventReceiver> receivers)
        {
            _receivers = receivers.ToList();
        }

        public void Handle(object testEvent)
        {
            foreach (var receiver in _receivers) receiver.Handle(testEvent);
        }
    }
}