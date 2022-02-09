using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Coderr.Server.WebSite.Infrastructure.WebSockets
{
    interface IWebSocketHub
    {
        Task Process(CancellationToken contextRequestAborted, WebSocket webSocket, ClaimsPrincipal claimsPrincipal);
    }

    public class WebSocketConnection
    {
        private WebSocket _socket;
        private ClaimsPrincipal _claimsPrincipal;

        public WebSocketConnection(WebSocket socket, ClaimsPrincipal claimsPrincipal)
        {
            _socket = socket;
            _claimsPrincipal = claimsPrincipal;
        }
    }

    public class WebSocketHub : IWebSocketHub
    {
        private List<WebSocketConnection> _connections = new List<WebSocketConnection>();

        public async Task Process(CancellationToken contextRequestAborted, WebSocket webSocket,
            ClaimsPrincipal claimsPrincipal)
        {
            var connection = new WebSocketConnection(webSocket, claimsPrincipal);
            lock (_connections)
            {
                _connections.Add(connection);
            }

            while (!contextRequestAborted.IsCancellationRequested)
            {
                await Task.Delay(1000, contextRequestAborted);
            }

            lock (_connections)
            {
                _connections.Remove(connection);
            }
        }

        public void Send()
        {

        }
    }
}
