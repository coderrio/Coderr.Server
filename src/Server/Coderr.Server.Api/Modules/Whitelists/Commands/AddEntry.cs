namespace Coderr.Server.Api.Modules.Whitelists.Commands
{
    /// <summary>
    ///     Add a domain that may post error reports without using a shared secret (javascript applications)
    /// </summary>
    [Command]
    public class AddEntry
    {
        /// <summary>
        ///     Applications that the domain is allowed for.
        /// </summary>
        public int[] ApplicationIds { get; set; } = new int[0];

        /// <summary>
        ///     For instance <c>yourdomain.com</c>.
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     To manually specify which IP addresses the domain matches.
        /// </summary>
        public string[] IpAddresses { get; set; } = new string[0];
    }
}