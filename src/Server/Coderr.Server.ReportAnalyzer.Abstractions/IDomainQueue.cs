using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Coderr.Server.ReportAnalyzer.Abstractions
{
    /// <summary>
    /// PublishAsync messages into the domain queue
    /// </summary>
    public interface IDomainQueue
    {
        Task PublishAsync(ClaimsPrincipal principal, object message);
    }
}
