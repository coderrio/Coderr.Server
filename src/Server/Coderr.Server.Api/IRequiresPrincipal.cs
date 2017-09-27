using System.Security.Claims;

namespace codeRR.Server.Api
{
    /// <summary>
    ///     The handler requires the current identity to make sure that the user can perform the actions
    /// </summary>
    public interface IRequiresPrincipal
    {
        /// <summary>
        ///     Current user
        /// </summary>
        ClaimsPrincipal Principal { get; }
    }
}