using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using codeRR.Server.Api;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Accounts.Requests;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Core.Invitations.CommandHandlers
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
    [Component, UpdatesLoggedInAccount]
    public class AcceptInvitationHandler : IRequestHandler<AcceptInvitation, AcceptInvitationReply>, IRequiresPrincipal, IUpdatesPrincipal
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEventBus _eventBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AcceptInvitationHandler));
        private readonly IInvitationRepository _repository;
        private ClaimsPrincipal _principal;

        /// <summary>
        /// Creates a new instance of <see cref="AcceptInvitationHandler"/>.
        /// </summary>
        /// <param name="repository">invitation repos</param>
        /// <param name="accountRepository">To load inviter and invitee</param>
        /// <param name="eventBus">to publish <see cref="InvitationAccepted"/></param>
        public AcceptInvitationHandler(IInvitationRepository repository,
            IAccountRepository accountRepository, IEventBus eventBus)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _eventBus = eventBus;
        }

        public ClaimsPrincipal Principal
        {
            get { return _principal ?? ClaimsPrincipal.Current; }
            set { _principal = value; }
        }

        /// <inheritdoc />
        public async Task<AcceptInvitationReply> ExecuteAsync(AcceptInvitation request)
        {
            var invitation = await _repository.GetByInvitationKeyAsync(request.InvitationKey);
            if (invitation == null)
            {
                _logger.Error("Failed to find invitation key" + request.InvitationKey);
                return null;
            }
            await _repository.DeleteAsync(request.InvitationKey);

            Account account;
            if (request.AccountId == 0)
            {
                account = new Account(request.UserName, request.Password);
                account.SetVerifiedEmail(request.AcceptedEmail);
                account.Activate();
                account.Login(request.Password);
                await _accountRepository.CreateAsync(account);
            }
            else
            {
                account = await _accountRepository.GetByIdAsync((int) request.AccountId);
                account.SetVerifiedEmail(request.AcceptedEmail);
            }

            var inviter = await _accountRepository.FindByUserNameAsync(invitation.InvitedBy);

            if (Principal.IsAccount(account.Id))
            {
                var claims = invitation.Invitations
                    .Select(
                        x => new Claim(OneTrueClaims.Application, x.ApplicationId.ToString(), ClaimValueTypes.Integer32))
                    .ToList();

                var context = new PrincipalFactoryContext(account.Id, account.UserName, new string[0])
                {
                    Claims = claims.ToArray(),
                    AuthenticationType = "Invite"
                };
                var principal = await PrincipalFactory.CreateAsync(context);
                principal.AddUpdateCredentialClaim();
                Thread.CurrentPrincipal = principal;
                Principal = principal;
            }

            // Account have not been created before the invitation was accepted.
            if (request.AccountId == 0)
            {
                await _eventBus.PublishAsync(new AccountRegistered(account.Id, account.UserName));
                await _eventBus.PublishAsync(new AccountActivated(account.Id, account.UserName)
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
            await _eventBus.PublishAsync(e);

            return new AcceptInvitationReply(account.Id, account.UserName);
        }
    }
}