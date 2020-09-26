using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Events;
using Coderr.Server.Api.Core.Accounts.Requests;
using Coderr.Server.Domain.Core.Account;

namespace Coderr.Server.App.Core.Accounts
{
    public interface IAccountService
    {
        /// <summary>
        ///     Accepts and deletes the invitation. Sends an event which is picked up by the application domain (which transforms
        ///     the pending invite to a membership)
        /// </summary>
        /// <param name="messagingPrincipal">Logged in user.</param>
        /// <param name="request">Information about the accept invitation request</param>
        /// <returns>Updated identity with permission to the applications that the user was invited to.</returns>
        /// <remarks>
        ///     <para>
        ///         Do note that an invitation can be accepted by using another email address than the one that the invitation was
        ///         sent to. So take care
        ///         when handling the <see cref="InvitationAccepted" /> event. Update the email that as used when sending the
        ///         invitation.
        ///     </para>
        /// </remarks>
        Task<ClaimsIdentity> AcceptInvitation(ClaimsPrincipal messagingPrincipal, AcceptInvitation request);

        /// <summary>
        /// Validate login information (check if the specified information is available).
        /// </summary>
        /// <param name="emailAddress">Email address as specified by the user.</param>
        /// <param name="userName">Wanted userName</param>
        /// <returns></returns>
        Task<ValidateNewLoginReply> ValidateLogin(string emailAddress, string userName);

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="activationKey">Key from sent email</param>
        /// <param name="newPassword">password</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        Task<bool> ResetPassword(string activationKey, string newPassword);

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="user">Currently logged in user (if logged in)</param>
        /// <param name="activationKey">Activation key</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        Task<ClaimsIdentity> ActivateAccount(ClaimsPrincipal user, string activationKey);

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="userName">Entered user name</param>
        /// <param name="password">clear text password</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        Task<ClaimsIdentity> Login(string userName, string password);

        /// <summary>
        /// Create a new account for the given userName
        /// </summary>
        /// <param name="userName"></param>
        Task<Account> CreateAsync(string userName, string email);
    }
}