using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using DotNetCqs;
using DotNetCqs.MessageProcessor;

namespace Coderr.Server.WebSite.Infrastructure.Cqs.Adapters
{
    [ContainerService(RegisterAsSelf = true)]
    public class ExecuteDirectlyMessageBus : IMessageBus
    {
        private readonly IMessageBus _outboundBus;
        private readonly IMessageInvoker _messageInvoker;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        public ExecuteDirectlyMessageBus(IMessageInvoker messageInvoker, IMessageBus outboundBus)
        {
            _messageInvoker = messageInvoker;
            _outboundBus = outboundBus;
        }

        public async Task SendAsync(ClaimsPrincipal principal, object message)
        {
            await _lock.WaitAsync();
            try
            {
                var outboundMessages = new List<Message>();
                var ctx = new InvocationContext("Directly", principal, _messageInvoker, outboundMessages);
                await _messageInvoker.ProcessAsync(ctx, message);
                foreach (var outboundMessage in outboundMessages)
                    await _outboundBus.SendAsync(principal, outboundMessage);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SendAsync(ClaimsPrincipal principal, Message message)
        {
            await _lock.WaitAsync();
            try
            {
                var outboundMessages = new List<Message>();
                var ctx = new InvocationContext("Directly", principal, _messageInvoker, outboundMessages);
                await _messageInvoker.ProcessAsync(ctx, message);
                foreach (var outboundMessage in outboundMessages) await _outboundBus.SendAsync(principal, outboundMessage);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SendAsync(Message message)
        {
            await _lock.WaitAsync();
            try
            {
                var outboundMessages = new List<Message>();
                var ctx = new InvocationContext("Directly", null, _messageInvoker, outboundMessages);
                await _messageInvoker.ProcessAsync(ctx, message);
                foreach (var outboundMessage in outboundMessages) await _outboundBus.SendAsync(outboundMessage);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task SendAsync(object message)
        {
            await _lock.WaitAsync();
            try
            {
                var outboundMessages = new List<Message>();
                var ctx = new InvocationContext("Directly", null, _messageInvoker, outboundMessages);
                await _messageInvoker.ProcessAsync(ctx, message);
                foreach (var outboundMessage in outboundMessages) await _outboundBus.SendAsync(outboundMessage);
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}