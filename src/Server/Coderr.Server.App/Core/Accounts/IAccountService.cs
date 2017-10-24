using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Accounts.Requests;

namespace codeRR.Server.App.Core.Accounts
{
    public interface IAccountService
    {
        /// <summary>
        ///     Accepts and deletes the invitation. Sends an event which is picked up by the application domain (which transforms
        ///     the pending invite to a membership)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Do note that an invitation can be accepted by using another email address than the one that the invitation was
        ///         sent to. So take care
        ///         when handling the <see cref="InvitationAccepted" /> event. Update the email that as used when sending the
        ///         invitation.
        ///     </para>
        /// </remarks>
        Task<ClaimsIdentity> AcceptInvitation(ClaimsPrincipal user, AcceptInvitation request);

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
    }
}