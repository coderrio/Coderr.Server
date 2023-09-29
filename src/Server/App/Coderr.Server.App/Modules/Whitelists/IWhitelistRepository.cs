using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Coderr.Server.App.Modules.Whitelists
{
    /// <summary>
    ///     Whitelists is used for reports that don't use a shared secret
    /// </summary>
    public interface IWhitelistRepository
    {
        Task<WhitelistedDomainIp> FindIp(int applicationId, IPAddress address);
        Task<IReadOnlyList<Whitelist>> FindWhitelists(int applicationId);

        Task SaveIp(WhitelistedDomainIp entry);
    }
}