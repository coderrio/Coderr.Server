namespace Coderr.Server.App.Modules.Whitelists
{
    /// <summary>
    ///     Domain that is allowed to report errors without
    /// </summary>
    public class WhitelistedDomain
    {
        /// <summary>
        ///     Application that the domain is allowed to report for.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         <c>null</c> = all applications.
        ///     </para>
        /// </remarks>
        public int? ApplicationId { get; set; }

        /// <summary>
        ///     Domain name, must be an exact match. Can also be an IP address
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        ///     PK
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Addresses that have been stored for this domain
        /// </summary>
        public WhitelistedDomainIp[] IpAddresses { get; set; }
    }
}