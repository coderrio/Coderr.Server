using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;

namespace OneTrueError.Infrastructure.Security
{
    /// <summary>
    ///     Our OneTrueError specific extensions for claims handling.
    /// </summary>
    public static class ClaimsExtensions
    {
        /// <summary>
        ///     Mark the current principal so that ASP.NET Identity can get updated (to persist changes over request boundary).
        /// </summary>
        /// <param name="principal">current principal</param>
        public static void AddUpdateCredentialClaim(this ClaimsPrincipal principal)
        {
            principal.Identities.First().AddClaim(OneTrueClaims.UpdateIdentity);
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
        public static bool IsAccount(this IPrincipal principal, int accountId)
        {
            var p = principal as ClaimsPrincipal;
            if (p == null)
                return false;

            if (!p.Identity.IsAuthenticated)
                return false;

            return accountId == p.GetAccountId();
        }

        /// <summary>
        ///     Get if the <c>OneTrueClaims.ApplicationAdmin</c> claim is specified for the given application (claim value)
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns><c>true</c> if the claim was found with the given value; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static bool IsApplicationAdmin(this ClaimsPrincipal principal, int applicationId)
        {
            return
                principal.FindFirst(
                    x => (x.Type == OneTrueClaims.ApplicationAdmin) && (x.Value == applicationId.ToString())) != null;
        }

        /// <summary>
        ///     Get if the <c>OneTrueClaims.Application</c> claim is specified for the given application (claim value)
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns><c>true</c> if the claim was found with the given value; otherwise <c>false</c>.</returns>
        /// <exception cref="InvalidOperationException">Claim is not found in the identity.</exception>
        public static bool IsApplicationMember(this ClaimsPrincipal principal, int applicationId)
        {
            return
                principal.FindFirst(x => (x.Type == OneTrueClaims.Application) && (x.Value == applicationId.ToString())) !=
                null;
        }

        /// <summary>
        ///     Get if the user is part of <c>OneTrueClaims.RoleSysAdmin</c>.
        /// </summary>
        /// <param name="principal">principal to search in</param>
        /// <returns><c>true</c> if the role was found; otherwise <c>false</c>.</returns>
        public static bool IsSysAdmin(this IPrincipal principal)
        {
            return principal.IsInRole(OneTrueClaims.RoleSysAdmin);
        }
    }
}