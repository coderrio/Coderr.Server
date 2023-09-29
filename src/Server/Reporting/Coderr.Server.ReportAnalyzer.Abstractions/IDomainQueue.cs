using System.Security.Claims;
using System.Threading.Tasks;

namespace Coderr.Server.ReportAnalyzer.Abstractions
{
    /// <summary>
    ///     PublishAsync messages into the domain queue
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used by report analyzers to send commands/events into the queue that the application uses.
    ///     </para>
    /// </remarks>
    public interface IDomainQueue : ISaveable
    {
        Task PublishAsync(ClaimsPrincipal principal, object message);
    }
}