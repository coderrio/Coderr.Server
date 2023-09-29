using System;
using System.Net;

namespace Coderr.Server.App.Modules.Whitelists
{
    /// <summary>
    ///     IP address that we have looked up (DNS lookup) for the specified domain entry.
    /// </summary>
    public class WhitelistedDomainIp
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Domain that this entry is for.
        /// </summary>
        public int DomainId { get; set; }

        /// <summary>
        ///     Address that we found
        /// </summary>
        public IPAddress IpAddress { get; set; }

        /// <summary>
        ///     How this IP should be treated
        /// </summary>
        public IpType IpType { get; set; }

        /// <summary>
        ///     When this entry was stored.
        /// </summary>
        public DateTime StoredAtUtc { get; set; }
    }
}