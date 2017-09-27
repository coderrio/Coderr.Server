using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Accounts.Requests;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.App.Core.Invitations;
using codeRR.Server.App.Core.Invitations.CommandHandlers;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace codeRR.Server.App.Tests.Core.Invitations.Commands
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
        public async Task Should_delete_invitation_when_its_accepted_to_prevent_creating_multiple_accounts_with_the_same_invitation_key()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _sut.Principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.ExecuteAsync(request);

            AssertionExtensions.Should((object) actual).NotBeNull();
        }

        [Fact]
        public async Task Should_notify_system_of_the_accepted_invitation()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _sut.Principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.ExecuteAsync(request);

            await _eventBus.Received().PublishAsync(Arg.Any<InvitationAccepted>());
            var evt = _eventBus.Method("PublishAsync").Arg<InvitationAccepted>();
            AssertionExtensions.Should((string) evt.AcceptedEmailAddress).Be(request.AcceptedEmail);
            AssertionExtensions.Should((int) evt.AccountId).Be(InvitedAccountId);
            AssertionExtensions.Should((int) evt.ApplicationIds[0]).Be(1);
            AssertionExtensions.Should((string) evt.UserName).Be(_invitedAccount.UserName);
        }

        [Fact]
        public async Task Should_create_an_Account_for_invites_to_new_users()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            _sut.Principal = PrincipalHelper.Create(52, "arne");


            var actual = await _sut.ExecuteAsync(request);

            await _accountRepository.Received().CreateAsync(Arg.Any<Account>());
            var evt = _eventBus.Method("PublishAsync").Arg<InvitationAccepted>();
            AssertionExtensions.Should((int) evt.AccountId).Be(52);
        }

        [Fact]
        public async Task Should_publish_AccountRegistered_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            _sut.Principal = PrincipalHelper.Create(52, "arne");


            await _sut.ExecuteAsync(request);

            await _eventBus.Received().PublishAsync(Arg.Any<AccountRegistered>());
            var evt = _eventBus.Method("PublishAsync").Arg<AccountRegistered>();
            AssertionExtensions.Should((int) evt.AccountId).Be(52);
        }

        [Fact]
        public async Task Should_publish_AccountActivated_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request = new AcceptInvitation("arne", "pass", invitation.InvitationKey) { AcceptedEmail = "arne@gauffin.com" };
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            _sut.Principal = PrincipalHelper.Create(52, "arne");


            await _sut.ExecuteAsync(request);

            await _eventBus.Received().PublishAsync(Arg.Any<AccountActivated>());
            var evt = _eventBus.Method("PublishAsync").Arg<AccountActivated>();
            AssertionExtensions.Should((int) evt.AccountId).Be(52);
        }


        [Fact]
        public async Task Should_ignore_invitations_where_the_key_is_not_registered_in_the_db()
        {
            var request = new AcceptInvitation(InvitedAccountId, "invalid") { AcceptedEmail = "arne@gauffin.com" };
            _sut.Principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.ExecuteAsync(request);

            AssertionExtensions.Should((object) actual).BeNull();
        }
    }
}
