using System.Security.Principal;

namespace codeRR.Server.SqlServer.Tools
{
    public interface ICustomerIdPrincipal : IPrincipal
    {
        int CustomerId { get; }
    }
}