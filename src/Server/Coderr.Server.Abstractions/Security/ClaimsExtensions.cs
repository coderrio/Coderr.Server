using System;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;

namespace Coderr.Server.Abstractions.Security
{
    /// <summary>
    ///     Our Coderr specific extensions for claims handling.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        ///     Throws if <see cref="IsApplicationAdmin" /> returns false.
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <param name="applicationId">application to check</param>
        /// <returns><c>true</c> if the claim was found with the given value; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static void EnsureApplicationAdmin(this ClaimsPrincipal principal, int applicationId)
        {
            if (!IsApplicationAdmin(principal, applicationId))
                throw new UnauthorizedAccessException(
                    $"User {principal.Identity.Name} is not authorized to manage application {applicationId}.");
        }

        /// <summary>
        ///     Get account id (<see cref="ClaimTypes.NameIdentifier" />).
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns>id</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity/ies.</exception>
        public static int GetAccountId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new InvalidOperationException(
                    "Failed to find ClaimTypes.NameIdentifier, user is probably not logged in.");

            return int.Parse(claim.Value);
        }

        /// <summary>
        ///     Get account id (<see cref="ClaimTypes.NameIdentifier" />).
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns>id</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity/ies.</exception>
        public static int GetAccountId(this IPrincipal principal)
        {
            var prince = principal as ClaimsPrincipal;
            if (prince == null)
                throw new AuthenticationException(principal + " is not a ClaimsPrincipal.");

            var claim = prince.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new InvalidOperationException(
                    "Failed to find ClaimTypes.NameIdentifier, user is probably not logged in.");

            return int.Parse(claim.Value);
        }

        /// <summary>
        ///     Get account id (<see cref="ClaimTypes.NameIdentifier" />).
        /// </summary>
        /// <param name="identity">identity to search in</param>
        /// <returns>id</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static int GetAccountId(this ClaimsIdentity identity)
        {
            var claim = identity.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new InvalidOperationException(
                    "Failed to find ClaimTypes.NameIdentifier, user is probably not logged in.");

            return int.Parse(claim.Value);
        }

        /// <summary>
        ///     Checks if the currently logged in user is the same as the given id.
        /// </summary>
        /// <param name="principal">Some kind of principal</param>
        /// <param name="accountId">AccountId to compare with.</param>
        /// <returns>
        ///     <c>true</c> if current principal is a <c>ClaimsPrincipal</c>, the user is authenticated and the accountId is
        ///     same; otherwise <c>false</c>.
        /// </returns>
        public static bool IsCurrentAccount(this IPrincipal principal, int accountId)
        {
            var p = principal as ClaimsPrincipal;
            if (p == null)
                return false;

            if (!p.Identity.IsAuthenticated)
                return false;

            return accountId == p.GetAccountId();
        }

        /// <summary>
        ///     Checks if the user has the <c>CoderrClaims.ApplicationAdmin</c> claim or if user is SysAdmin or System.
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <param name="applicationId">Application to check</param>
        /// <returns><c>true</c> if the claim was found with the given value; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static bool IsApplicationAdmin(this ClaimsPrincipal principal, int applicationId)
        {
            return principal.HasClaim(CoderrClaims.ApplicationAdmin, applicationId.ToString())
                   || principal.IsInRole(CoderrRoles.SysAdmin)
                   || principal.IsInRole(CoderrRoles.System);
        }

        /// <summary>
        ///     Get if the <c>CoderrClaims.Application</c> claim is specified for the given application (claim value)
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <param name="applicationId">App to check</param>
        /// <param name="checkSystemRoles">Check if user is in role SysAdmin or if the user is the System.</param>
        /// <returns><c>true</c> if the claim was found with the given value; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static bool IsApplicationMember(this ClaimsPrincipal principal, int applicationId, bool checkSystemRoles = false)
        {
            var isAdmin = principal.HasClaim(CoderrClaims.Application, applicationId.ToString());
            if (checkSystemRoles)
                isAdmin = isAdmin || IsSysAdmin(principal) || principal.IsInRole(CoderrRoles.System);
            return isAdmin;
        }

        /// <summary>
        ///     Get if the user is part of <c>CoderrRoles.SysAdmin</c>.
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns><c>true</c> if the role was found; otherwise <c>false</c>.</returns>
        public static bool IsSysAdmin(this IPrincipal principal)
        {
            return principal.IsInRole(CoderrRoles.SysAdmin);
        }

        public static string ToFriendlyString(this IPrincipal principal)
        {
            var cc = principal as ClaimsPrincipal;
            if (cc == null || !principal.Identity.IsAuthenticated)
            {
                return "Anonymous";
            }

            string str = cc.Identity.Name + " [";
            foreach (var claim in cc.Claims)
            {
                var pos = claim.Type.LastIndexOf('/');
                if (pos != -1)
                    str += claim.Type.Substring(pos + 1);
                else
                    str += claim.Type;

                str += "=" + claim.Value + ", ";
            }
            str = str.Remove(str.Length - 2, 2) + "]";
            return str;
        }
    }
}