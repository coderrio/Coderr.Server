using System.Security.Claims;

namespace Coderr.Server.Abstractions.Security
{
    /// <summary>
    /// Used to move principal from the queue to the current container scope so that a proper DB connection can be opened.
    /// </summary>
    public interface IPrincipalAccessor
    {
        ClaimsPrincipal Principal { get; set; }
    }
}
