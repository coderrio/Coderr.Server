using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Accounts.Requests;
using OneTrueError.App.Core.Accounts;

namespace OneTrueError.App.Core.Invitations.CommandHandlers
{
    [Component]
    internal class AcceptInvitationHandler : IRequestHandler<AcceptInvitation, AcceptInvitationReply>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IEventBus _eventBus;
        private readonly IInvitationRepository _repository;

        public AcceptInvitationHandler(IInvitationRepository repository,
            IAccountRepository accountRepository, IEventBus eventBus)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _eventBus = eventBus;
        }

        public async Task<AcceptInvitationReply> ExecuteAsync(AcceptInvitation request)
        {
            var account = new Account(request.UserName, request.Password);
            account.SetVerifiedEmail(request.Email);

            var invitation = await _repository.FindByEmailAsync(request.Email);
            if (invitation == null)
            {
                return null;
            }
            await _repository.DeleteAsync(request.InvitationKey);
            var inviter = await _accountRepository.FindByUserNameAsync(invitation.InvitedBy);
            account.Activate();
            account.Login(request.Password);

            var roles=  invitation.Invitations.Select(x => "Member_" + x.ApplicationId).ToArray();
            await _accountRepository.CreateAsync(account);

            
            Thread.CurrentPrincipal = new OneTruePrincipal(account.Id, account.UserName, roles);
            await _eventBus.PublishAsync(new AccountActivated(account.Id, account.UserName)
            {
                EmailAddress = account.Email
            });
            var e = new InvitationAccepted(account.Id, invitation.InvitedBy, account.UserName)
            {
                EmailAddress = account.Email,
                ApplicationIds = invitation.Invitations.Select(x => x.ApplicationId).ToArray()
            };
            await _eventBus.PublishAsync(e);

            return new AcceptInvitationReply(account.Id, account.UserName);
        }
    }
}