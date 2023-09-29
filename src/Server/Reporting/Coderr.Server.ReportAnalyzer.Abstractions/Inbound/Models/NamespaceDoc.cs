using System.Runtime.CompilerServices;

namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Models
{
    /// <summary>
    ///     Used by the ReportReceiver to receive error reports and store them in the internal queue. Picked up by the report analyzer for analysis.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Do not modify these when the client library changes (unless the change is backwards compatible), instead create
    ///         a new class
    ///         which the changes are applied to and name it with the same version number that is sent by the client.
    ///     </para>
    ///     <para>
    ///         In that way we can support multiple API versions.
    ///     </para>
    /// </remarks>
    [CompilerGenerated]
    public class NamespaceDoc
    {
    }
}