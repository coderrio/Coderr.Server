using System.Data;
using System.Security.Claims;

namespace Coderr.Server.Abstractions
{
    public interface IConnectionFactory
    {
        IDbConnection OpenConnection(ClaimsPrincipal principal);
        
    }
}