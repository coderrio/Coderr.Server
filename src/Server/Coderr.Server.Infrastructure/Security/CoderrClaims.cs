using System.Security.Claims;

namespace codeRR.Server.Infrastructure.Security
{
    public class CoderrClaims
    {
        public const string CurrentApplicationId = "http://coderrapp.com/claims/currentapplicationid";
        public const string Application = "http://coderrapp.com/claims/application";
        public const string ApplicationName = "http://coderrapp.com/claims/application/name";
        public const string ApplicationAdmin = "http://coderrapp.com/claims/application/admin";

        public const string RoleSysAdmin = "SysAdmin";
        public const string RoleUser = "SysAdmin";
        public const string RoleSystem = "System";

        public static readonly Claim UpdateIdentity = new Claim("UpdateIdentity", "true");
    }
}