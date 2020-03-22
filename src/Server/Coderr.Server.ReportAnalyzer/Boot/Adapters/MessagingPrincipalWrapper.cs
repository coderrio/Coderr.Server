using System.Security.Claims;
using Coderr.Server.Abstractions.Security;

namespace Coderr.Server.ReportAnalyzer.Boot.Adapters
{
    /// <summary>
    ///     This principal should always be specified by the queue listeners when a new message is received.
    /// </summary>
    internal class MessagingPrincipalWrapper : IPrincipalAccessor
    {
        public ClaimsPrincipal Principal { get; set; }
        public ClaimsPrincipal FindPrincipal()
        {
            return Principal;
        }
    }
}