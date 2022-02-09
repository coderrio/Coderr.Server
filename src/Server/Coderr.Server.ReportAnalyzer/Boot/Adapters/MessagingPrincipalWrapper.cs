using System;
using System.Security.Claims;
using Coderr.Server.Abstractions.Security;
using DotNetCqs.Logging;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    /// <summary>
    ///     This principal should always be specified by the queue listeners when a new message is received.
    /// </summary>
    internal class MessagingPrincipalWrapper : IPrincipalAccessor
    {
        private ILog _logger = LogManager.GetLogger(typeof(MessagingPrincipalWrapper));

        public MessagingPrincipalWrapper()
        {
        }

        public ClaimsPrincipal Principal { get; set; }

        public ClaimsPrincipal FindPrincipal()
        {
            return Principal;
        }
    }
}