using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Abstractions;
using DotNetCqs.Bus;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    /// <summary>
    ///     Publishes events in the other bounded context (the application context)
    /// </summary>
    internal class DomainQueueWrapper3 : IDomainQueue
    {
        private readonly ScopedMessageBus _messageBus;
        private bool _gotMessages;

        public DomainQueueWrapper3(ScopedMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        public async Task PublishAsync(ClaimsPrincipal principal, object message)
        {
            await _messageBus.SendAsync(principal, message);
            _gotMessages = true;
        }


        public async Task SaveChanges()
        {
            if (_gotMessages)
                await _messageBus.CommitAsync();
        }
    }
}