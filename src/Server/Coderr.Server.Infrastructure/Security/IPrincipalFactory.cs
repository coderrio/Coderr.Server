using System.Security.Claims;
using System.Threading.Tasks;

namespace codeRR.Server.Infrastructure.Security
{
    /// <summary>
    ///     Allows us to create a claims principal without a dependency to ASP.NET Identity.
    /// </summary>
    public interface IPrincipalFactory
    {
        /// <summary>
        ///     Create a new principal.
        /// </summary>
        /// <param name="context">Account information etc</param>
        /// <returns>Principal</returns>
        Task<ClaimsPrincipal> CreateAsync(PrincipalFactoryContext context);
    }
}