namespace Coderr.Server.Api.Modules.Whitelists.Commands
{
    /// <summary>
    ///     Add a domain that may post error reports without using a shared secret (javascript applications)
    /// </summary>
    public class AddDomain
    {
        /// <summary>
        ///     Application that the domain is allowed for.
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        ///     For instance <c>yourdomain.com</c>.
        /// </summary>
        public string DomainName { get; set; }
    }
}