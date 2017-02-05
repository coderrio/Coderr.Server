using System.Threading.Tasks;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Accounts.Requests;
using OneTrueError.App.Core.Accounts;
using OneTrueError.App.Core.Invitations;
using OneTrueError.App.Core.Invitations.CommandHandlers;
using Xunit;

namespace OneTrueError.App.Tests.Core.Invitations.Commands
{
    public class AcceptInvitationHandlerTests
    {
        private IInvitationRepository _repository;
        private IAccountRepository _accountRepository;
        private IEventBus _eventBus;
        private AcceptInvitationHandler _sut;
        private Account _invitedAccount;
        private const int InvitedAccountId = 999;


        public AcceptInvitationHandlerTests()
        {
            _repository = Substitute.For<IInvitationRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _eventBus = Substitute.For<IEventBus>();
            _sut = new AcceptInvitationHandler(_repository, _accountRepository, _eventBus);
            _invitedAccount = new Account("arne", "1234");
            _invitedAccount.SetId(InvitedAccountId);
            _invitedAccount.SetVerifiedEmail("jonas@gauffin.com");
            _accountRepository.GetByIdAsync(InvitedAccountId).Returns(_invitedAccount);
        }

        [Fact]
        public async Task should_delete_invitation_when_its_accepted_to_prevent_creating_multiple_accounts_with_the_same_invitation_key()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);

            var actual = await _sut.ExecuteAsync(request);

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task should_notify_system_of_the_accepted_invitation()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);

            var actual = await _sut.ExecuteAsync(request);

            _eventBus.Received().PublishAsync(Arg.Any<InvitationAccepted>());
            var evt = _eventBus.Method("PublishAsync").Arg<InvitationAccepted>();
            evt.AcceptedEmailAddress.Should().Be(request.AcceptedEmail);
            evt.AccountId.Should().Be(InvitedAccountId);
            evt.ApplicationIds[0].Should().Be(1);
            evt.UserName.Should().Be(_invitedAccount.UserName);
        }

        [Fact]
        public async Task should_create_an_Account_for_invites_to_new_users()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));


            var actual = await _sut.ExecuteAsync(request);

            _accountRepository.Received().CreateAsync(Arg.Any<Account>());
            var evt = _eventBus.Method("PublishAsync").Arg<InvitationAccepted>();
            evt.AccountId.Should().Be(52);
        }

        [Fact]
        public async Task should_publish_AccountRegistered_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));


            await _sut.ExecuteAsync(request);

            _eventBus.Received().PublishAsync(Arg.Any<AccountRegistered>());
            var evt = _eventBus.Method("PublishAsync").Arg<AccountRegistered>();
            evt.AccountId.Should().Be(52);
        }

        [Fact]
        public async Task should_publish_AccountActivated_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));


            await _sut.ExecuteAsync(request);

            _eventBus.Received().PublishAsync(Arg.Any<AccountActivated>());
            var evt = _eventBus.Method("PublishAsync").Arg<AccountActivated>();
            evt.AccountId.Should().Be(52);
        }


        [Fact]
        public async Task should_ignore_invitations_where_the_key_is_not_registered_in_the_db()
        {
            var request = new AcceptInvitation(InvitedAccountId, "invalid") { AcceptedEmail = "arne@gauffin.com" };

            var actual = await _sut.ExecuteAsync(request);

            actual.Should().BeNull();
        }
    }
}
