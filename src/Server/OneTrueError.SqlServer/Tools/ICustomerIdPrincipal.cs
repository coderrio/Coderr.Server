using System.Security.Principal;

namespace OneTrueError.SqlServer.Tools
{
    public interface ICustomerIdPrincipal : IPrincipal
    {
        int CustomerId { get; }
    }
}
