namespace Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Whitelists.Commands
{
    /// <summary>
    ///     Add a domain that may post error reports without using a shared secret (javascript applications)
    /// </summary>
    public class AddDomain
    {
        /// <summary>
        ///     Application that the domain is allowed for.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         ´<c>null if allowed for all applications</c>
        ///     </para>
        /// </remarks>
        public int? ApplicationId { get; set; }

        /// <summary>
        ///     For instance <c>yourdomain.com</c>.
        /// </summary>
        public string DomainName { get; set; }
    }
}