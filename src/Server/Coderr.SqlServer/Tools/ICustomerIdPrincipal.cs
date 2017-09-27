using System.Security.Principal;

namespace codeRR.SqlServer.Tools
{
    public interface ICustomerIdPrincipal : IPrincipal
    {
        int CustomerId { get; }
    }
}