using System.Net;
using System.Threading.Tasks;

namespace Coderr.Server.App.Modules.Whitelists
{
    /// <summary>
    ///     Used to validate origin of inbound requests when a shared secret is not used.
    /// </summary>
    public interface IWhitelistService
    {
        /// <summary>
        ///     Is domain white listed?
        /// </summary>
        /// <param name="appKey">AppKey used when receiving error reports.</param>
        /// <param name="remoteAddress">IP address of the client reporting the error.</param>
        /// <returns></returns>
        Task<bool> Validate(string appKey, IPAddress remoteAddress);

        /// <summary>
        ///     Is domain white listed?
        /// </summary>
        /// <param name="applicationId">Application that the error is reported for.</param>
        /// <param name="remoteAddress">IP address of the client reporting the error.</param>
        /// <returns></returns>
        Task<bool> Validate(int applicationId, IPAddress remoteAddress);
    }
}