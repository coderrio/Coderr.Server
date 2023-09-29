using System.Security.Claims;

namespace Coderr.Server.Abstractions.Security
{
    public class CoderrClaims
    {
        public const string Application = "http://coderr/claims/application";
        public const string ApplicationAdmin = "http://coderr/claims/application/admin";

        public static readonly ClaimsPrincipal SystemPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "0"),
            new Claim(ClaimTypes.Name, "System"),
            new Claim(ClaimTypes.Role, CoderrRoles.System)
        }));
    }
}