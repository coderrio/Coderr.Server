using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Api;
using Coderr.Server.Infrastructure.Messaging;
using Coderr.Server.WebSite.Infrastructure.Cqs;
using DotNetCqs;
using DotNetCqs.Queues;
using Microsoft.AspNetCore.SignalR;

namespace Coderr.Server.WebSite.Hubs
{
    public class WebSocketHub : Hub<CoderrHub>, IMessageQueue
    {
        public WebSocketHub(IRouteRegistrar registrar)
        {
            registrar.RegisterAppQueue(this);
            registrar.RegisterReportQueue(this);
        }

        public IMessageQueueSession BeginSession()
        {
            return new HubSession(this);
        }

        public override Task OnConnectedAsync()
        {
            Clients.Caller.OnEvent(new HubEvent()
            {
                TypeName = "HelloWorld",
                Body = "Hello the world!"
            });
            return base.OnConnectedAsync();
        }

        public string Name { get; } = "WebSocket";

        public async Task Send(ClaimsPrincipal principal, IReadOnlyList<Message> messages)
        {
            var eventMessages = messages
                .Where(x => x.GetType().GetCustomAttribute<CommandAttribute>() == null &&
                            !RegisterCqsServices.IsQuery(x))
                .ToList();
            if (principal == null)
            {
                return;
            }

            var sender = Clients.All;

            foreach (var message in eventMessages)
            {
                await sender.OnEvent(new HubEvent
                {
                    CorrelationId = message.CorrelationId,
                    Body = message.Body,
                    TypeName = message.Body.GetType().Name
                });
            }
        }
    }
}