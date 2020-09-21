using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts;
using DotNetCqs;
using DotNetCqs.Queues;
using log4net;

namespace Coderr.Server.ReportAnalyzer.Boot.Starters
{
    /// <summary>
    /// Used to decide which queue outbound messages should be enqueued in.
    /// </summary>
    internal class MessageRouter : IMessageRouter
    {
        private ILog _logger = LogManager.GetLogger(typeof(MessageRouter));
        private readonly Assembly _appAssembly;

        public MessageRouter()
        {
            _appAssembly = typeof(RegisterSimple).Assembly;
        }

        /// <summary>
        /// Queue where all messages defined in App.Api is processed
        /// </summary>
        public IMessageQueue AppQueue { get; set; }

        /// <summary>
        /// Queue where all report analyzer messages are being processed.
        /// </summary>
        public IMessageQueue ReportAnalyzerQueue { get; set; }

        public async Task SendAsync(Message message)
        {
            await SendAsync(null, message);
        }


        public async Task SendAsync(IReadOnlyCollection<Message> messages)
        {
            await SendAsync(null, messages);
        }

        public async Task SendAsync(ClaimsPrincipal principal, Message message)
        {
            if (IsReportAnalyzerMessage(message))
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

        private bool IsReportAnalyzerMessage(Message message)
        {
            return message.Body?.GetType().Assembly != _appAssembly;
        }

        private async Task SendAppMessage(ClaimsPrincipal claimsPrincipal, Message message)
        {
            using (var session = AppQueue.BeginSession())
            {
                if (claimsPrincipal == null)
                    await session.EnqueueAsync(message);
                else
                    await session.EnqueueAsync(claimsPrincipal, message);
                _logger.Info($"AppMsg[{GetHashCode()}]: {message.Body}");
                await session.SaveChanges();
            }
        }

        private async Task SendAppMessages(ClaimsPrincipal principal, IReadOnlyCollection<Message> msgs)
        {
            if (msgs.Count == 0)
                return;

            using (var session = AppQueue.BeginSession())
            {
                foreach (var message in msgs)
                {
                    _logger.Debug($"AppMsg[{GetHashCode()}]: {message.Body}");
                }
                await session.EnqueueAsync(principal, msgs);
                await session.SaveChanges();
            }
        }

        private async Task SendReportAnalyzerMessage(ClaimsPrincipal claimsPrincipal, Message message)
        {
            using (var session = ReportAnalyzerQueue.BeginSession())
            {
                if (claimsPrincipal == null)
                    await session.EnqueueAsync(message);
                else
                    await session.EnqueueAsync(claimsPrincipal, message);
                _logger.Debug($"AnalyzerMsg[{GetHashCode()}]: {message.Body}");
                await session.SaveChanges();
            }
        }

        private async Task SendReportAnalyzerMessages(ClaimsPrincipal principal, IReadOnlyCollection<Message> msgs)
        {
            if (msgs.Count == 0)
                return;

            using (var session = ReportAnalyzerQueue.BeginSession())
            {
                foreach (var message in msgs)
                {
                    _logger.Debug($"AnalyzerMsg[{GetHashCode()}]: {message.Body}");
                }
                await session.EnqueueAsync(principal, msgs);
                await session.SaveChanges();
            }
        }
    }
}