using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Abstractions.Security;
using DotNetCqs;
using DotNetCqs.Queues;
using log4net;

namespace Coderr.Server.Infrastructure.Messaging
{
    /// <summary>
    ///     Used to decide which queue outbound messages should be enqueued in.
    /// </summary>
    public class MessageRouter : IMessageRouter, IRouteRegistrar
    {
        public static readonly MessageRouter Instance = new MessageRouter();
        private readonly List<IMessageQueue> _appQueues = new List<IMessageQueue>();
        private readonly ILog _logger = LogManager.GetLogger(typeof(MessageRouter));
        private readonly List<IMessageQueue> _reportAnalyzerQueues = new List<IMessageQueue>();

        private MessageRouter()
        {
        }

        async Task IMessageRouter.SendAsync(Message message)
        {
            await SendAsync(null, message);
        }


        async Task IMessageRouter.SendAsync(IReadOnlyCollection<Message> messages)
        {
            await SendAsync(null, messages);
        }

        public async Task SendAsync(ClaimsPrincipal principal, Message message)
        {
            if (!IsReportAnalyzerMessage(message))
                await SendAppMessage(principal, message);
            else
                await SendReportAnalyzerMessage(principal, message);
        }

        public async Task SendAsync(ClaimsPrincipal principal, IReadOnlyCollection<Message> messages)
        {
            var reporAnalyzerMessages = messages.Where(IsReportAnalyzerMessage).ToList();
            var appMsgs = messages.Except(reporAnalyzerMessages).ToList();
            await SendReportAnalyzerMessages(principal, reporAnalyzerMessages);
            await SendAppMessages(principal, appMsgs);
        }

        public void RegisterAppQueue(IMessageQueue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            // We have multiple places now that can register a queue.
            // TODO: Clean that up ;)
            if (_appQueues.Any(existingQueue => existingQueue.Name == queue.Name)) return;

            _appQueues.Add(queue);
        }

        public void RegisterReportQueue(IMessageQueue queue)
        {
            if (queue == null) throw new ArgumentNullException(nameof(queue));

            // We have multiple places now that can register a queue.
            // TODO: Clean that up ;)
            if (_reportAnalyzerQueues.Any(existingQueue => existingQueue.Name == queue.Name)) return;

            _reportAnalyzerQueues.Add(queue);
        }

        private static bool IsReportAnalyzerMessage(Message message)
        {
            return message?.Body?.GetType().Namespace?.Contains("ReportAnalyzer") == true;
        }

        private async Task SendAppMessage(ClaimsPrincipal claimsPrincipal, Message message)
        {
            if (claimsPrincipal == null)
                _logger.Warn("Null principal for " + message.Body.GetType() + " from " + Environment.StackTrace);

            foreach (var queue in _appQueues)
            {
                try
                {
                    using (var session = queue.BeginSession())
                    {
                        if (claimsPrincipal == null)
                            await session.EnqueueAsync(message);
                        else
                            await session.EnqueueAsync(claimsPrincipal, message);
                        _logger.Info($"AppMsg[{claimsPrincipal?.ToFriendlyString()}]: {message.Body}");
                        await session.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {queue.Name});
                }
            }
        }

        private async Task SendAppMessages(ClaimsPrincipal principal, List<Message> msgs)
        {
            if (principal == null)
                _logger.Warn("Null principal for " + msgs.FirstOrDefault()?.Body.GetType() + " from " +
                             Environment.StackTrace);


            foreach (var queue in _appQueues)
            {
                try
                {
                    using (var session = queue.BeginSession())
                    {
                        foreach (var message in msgs)
                            _logger.Debug($"AppMsg[{principal?.ToFriendlyString()}]: {message.Body}");
                        await session.EnqueueAsync(principal, msgs);
                        await session.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {queue.Name});
                }
            }
        }

        private async Task SendReportAnalyzerMessage(ClaimsPrincipal claimsPrincipal, Message message)
        {
            if (claimsPrincipal == null)
                _logger.Warn("Null principal for " + message.Body.GetType() + " from " + Environment.StackTrace);


            foreach (var queue in _reportAnalyzerQueues)
            {
                try
                {
                    using (var session = queue.BeginSession())
                    {
                        if (claimsPrincipal == null)
                            await session.EnqueueAsync(message);
                        else
                            await session.EnqueueAsync(claimsPrincipal, message);
                        _logger.Debug($"AnalyzerMsg[{claimsPrincipal?.ToFriendlyString()}]: {message.Body}");
                        await session.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {queue.Name});
                }
            }
        }

        private async Task SendReportAnalyzerMessages(ClaimsPrincipal principal, List<Message> msgs)
        {
            if (principal == null)
                _logger.Warn("Null principal for " + msgs.FirstOrDefault()?.Body.GetType() + " from " +
                             Environment.StackTrace);

            foreach (var queue in _reportAnalyzerQueues)
            {
                try
                {
                    using (var session = queue.BeginSession())
                    {
                        foreach (var message in msgs)
                            _logger.Debug($"AnalyzerMsg[{principal?.ToFriendlyString()}]: {message.Body}");
                        await session.EnqueueAsync(principal, msgs);
                        await session.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Err.Report(ex, new {queue.Name});
                }
            }
        }
    }
}