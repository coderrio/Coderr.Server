using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCqs;
using DotNetCqs.Queues;

namespace Coderr.Server.Web.Boot.Cqs
{
    public class RoutingMessageBus : IMessageBus
    {
        private readonly IMessageRouter _router;

        public RoutingMessageBus(IMessageRouter router)
        {
            _router = router;
        }

        public async Task SendAsync(ClaimsPrincipal principal, object message)
        {
            await SendAsync(principal, new Message(message));
        }

        public async Task SendAsync(ClaimsPrincipal principal, Message message)
        {
            await _router.SendAsync(principal, message);
        }

        public async Task SendAsync(Message message)
        {
            await _router.SendAsync(message);
        }

        public async Task SendAsync(object message)
        {
            await SendAsync(new Message(message));
        }
    }
}