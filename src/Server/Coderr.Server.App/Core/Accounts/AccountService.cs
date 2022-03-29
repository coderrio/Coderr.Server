using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Accounts.Events;
using Coderr.Server.Api.Core.Accounts.Requests;
using Coderr.Server.App.Core.Invitations;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Infrastructure.Security;

using DotNetCqs;
using log4net;

namespace Coderr.Server.App.Core.Accounts
{
    /// <summary>
    ///     This is a service and not CQS object as the methods here are of RPC type which isn't really a good fit for commands
    ///     or queries.
    /// </summary>
    [ContainerService]
    public class AccountService : IAccountService
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountService));
        private readonly IAccountRepository _repository;
        private readonly IMessageBus _messageBus;
        private readonly IApplicationRepository _applicationRepository;
        private IInvitationRepository _invitationRepository;

        public AccountService(IAccountRepository repository, IMessageBus messageBus, IApplicationRepository applicationRepository, IInvitationRepository invitationRepository)
        {
            _repository = repository;
            _messageBus = messageBus;
            _applicationRepository = applicationRepository;
            _invitationRepository = invitationRepository;
        }


        public async Task<ValidateNewLoginReply> ValidateLogin(string emailAddress, string userName)
        {
            var reply = new ValidateNewLoginReply();
            if (!string.IsNullOrEmpty(emailAddress))
                reply.EmailIsTaken = await _repository.IsEmailAddressTakenAsync(emailAddress);

            if (!string.IsNullOrEmpty(userName))
                reply.UserNameIsTaken = await _repository.FindByUserNameAsync(userName) != null;

            return reply;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     <c>false</c> if key was not found.
        /// </returns>
        public async Task<bool> ResetPassword(string activationKey, string newPassword)
        {
            var account = await _repository.FindByActivationKeyAsync(activationKey);
            if (account == null)
                return false;

            account.ChangePassword(newPassword);
            await _repository.UpdateAsync(account);
            return true;
        }


        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="messagingPrincipal"></param>
        /// <param name="activationKey"></param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<ClaimsIdentity> ActivateAccount(ClaimsPrincipal messagingPrincipal, string activationKey)
        {
            if (messagingPrincipal == null) throw new ArgumentNullException(nameof(messagingPrincipal));
            if (activationKey == null) throw new ArgumentNullException(nameof(activationKey));

            var account = await _repository.FindByActivationKeyAsync(activationKey);
            if (account == null)
                throw new ArgumentOutOfRangeException(nameof(activationKey), activationKey,
                    "Key was not found.");

            account.Activate();
            await _repository.UpdateAsync(account);
            
            
            if (!messagingPrincipal.IsCurrentAccount(account.Id))
            {
                var evt = new AccountActivated(account.Id, account.UserName)
                {
                    EmailAddress = account.Email
                };
                await _messageBus.SendAsync(messagingPrincipal, evt);
            }


            var identity = await CreateIdentity(account.Id, account.UserName, account.IsSysAdmin);
            return identity;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<bool> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (!user.ValidatePassword(currentPassword))
                return false;

            user.ChangePassword(newPassword);
            await _repository.UpdateAsync(user);
            return true;
        }


        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<ClaimsIdentity> Login(string userName, string password)
        {
            if (userName == null) throw new ArgumentNullException(nameof(userName));
            if (password == null) throw new ArgumentNullException(nameof(password));

            var account = await _repository.FindByUserNameAsync(userName);

            try
            {
                if (account == null || !account.Login(password))
                {
                    _logger.Debug("Logging in " + userName);
                    await _messageBus.SendAsync(new Message(new LoginFailed(userName) {InvalidLogin = true}));
                    if (account != null)
                        await _repository.UpdateAsync(account);
                    return null;
                }
            }
            catch (AuthenticationException ex)
            {
                _logger.Debug("Logging failed for " + userName, ex);
                _messageBus.SendAsync(new Message(new LoginFailed(userName) {IsLocked = true})).Wait();
                throw;
            }

            await _repository.UpdateAsync(account);
            var identity= await CreateIdentity(account.Id, account.UserName, account.IsSysAdmin);
            return identity;
        }

        public async Task<Account> CreateAsync(string userName, string email)
        {
            var password = Guid.NewGuid().ToString("N").Substring(0, 10);
            var account = new Account(userName, password);
            account.Activate();
            account.SetVerifiedEmail(email);
            await _repository.CreateAsync(account);
            await _messageBus.SendAsync(new AccountRegistered(account.Id, userName));
            return account;
        }

        /// <summary>
        ///     Accepts and deletes the invitation. Sends an event which is picked up by the application domain (which transforms
        ///     the pending invite to a membership)
        /// </summary>
        /// <param name="messagingPrincipal">Principal that outbound messages should be sent with</param>
        /// <param name="request">accept invitation DTO</param>
        /// <remarks>
        ///     <para>
        ///         Do note that an invitation can be accepted by using another email address than the one that the invitation was
        ///         sent to. So take care
        ///         when handling the <see cref="InvitationAccepted" /> event. Update the email that as used when sending the
        ///         invitation.
        ///     </para>
        /// </remarks>
        public async Task<ClaimsIdentity> AcceptInvitation(ClaimsPrincipal messagingPrincipal, AcceptInvitation request)
        {
            var invitation = await _invitationRepository.GetByInvitationKeyAsync(request.InvitationKey);
            if (invitation == null)
            {
                _logger.Error("Failed to find invitation key" + request.InvitationKey);
                return null;
            }
            await _invitationRepository.DeleteAsync(request.InvitationKey);

            Account account;
            if (request.AccountId == 0)
            {
                account = new Account(request.UserName, request.Password);
                account.SetVerifiedEmail(request.AcceptedEmail);
                account.Activate();
                account.Login(request.Password);
                await _repository.CreateAsync(account);
            }
            else
            {
                account = await _repository.GetByIdAsync(request.AccountId);
                account.SetVerifiedEmail(request.AcceptedEmail);
            }

            var inviter = await _repository.FindByUserNameAsync(invitation.InvitedBy);

            ClaimsIdentity identity = null;
            identity = await CreateIdentity(account.Id, account.UserName, account.IsSysAdmin);

            // Account have not been created before the invitation was accepted.
            if (request.AccountId == 0)
            {
                await _messageBus.SendAsync(messagingPrincipal, new AccountRegistered(account.Id, account.UserName));
                await _messageBus.SendAsync(messagingPrincipal, new AccountActivated(account.Id, account.UserName)
                {
                    EmailAddress = account.Email
                });
            }

            var e = new InvitationAccepted(account.Id, invitation.InvitedBy, account.UserName)
            {
                InvitedEmailAddress = invitation.EmailToInvitedUser,
                AcceptedEmailAddress = request.AcceptedEmail,
                ApplicationIds = invitation.Invitations.Select(x => x.ApplicationId).ToArray()
            };
            await _messageBus.SendAsync(messagingPrincipal, e);

            return identity;
        }

        private async Task<ClaimsIdentity> CreateIdentity(int accountId, string userName, bool isSysAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String)
            };

            var apps = await _applicationRepository.GetForUserAsync(accountId);
            foreach (var app in apps)
            {
                claims.Add(new Claim(CoderrClaims.Application, app.ApplicationId.ToString(), ClaimValueTypes.Integer32));
                if (app.IsAdmin)
                    claims.Add(new Claim(CoderrClaims.ApplicationAdmin, app.ApplicationId.ToString(), ClaimValueTypes.Integer32));
            }


            //accountId == 1 for backwards compatibility (with version 1.0)
            if (isSysAdmin || accountId == 1)
                claims.Add(new Claim(ClaimTypes.Role, CoderrRoles.SysAdmin));

            return new ClaimsIdentity(claims.ToArray(), AuthenticationTypes.Default);
        }
    }
}