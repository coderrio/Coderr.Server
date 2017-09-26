using System.Security.Claims;

namespace OneTrueError.Infrastructure.Security
{
    public class OneTrueClaims
    {
        public const string CurrentApplicationId = "http://onetrueerror.com/claims/currentapplicationid";
        public const string Application = "http://onetrueerror.com/claims/application";
        public const string ApplicationName = "http://onetrueerror.com/claims/application/name";
        public const string ApplicationAdmin = "http://onetrueerror.com/claims/application/admin";

        public const string RoleSysAdmin = "SysAdmin";
        public const string RoleUser = "SysAdmin";
        public const string RoleSystem = "System";

        public static readonly Claim UpdateIdentity = new Claim("UpdateIdentity", "true");
    }
}