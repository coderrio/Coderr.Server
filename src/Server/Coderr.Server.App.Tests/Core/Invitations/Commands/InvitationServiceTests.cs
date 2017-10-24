using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Accounts.Requests;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.App.Core.Invitations;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace codeRR.Server.App.Tests.Core.Invitations.Commands
{
    public class InvitationServiceTests
    {
        public InvitationServiceTests()
        {
            _repository = Substitute.For<IInvitationRepository>();
            _accountRepository = Substitute.For<IAccountRepository>();
            _applicationRepository = Substitute.For<IApplicationRepository>();
            _messageBus = Substitute.For<IMessageBus>();
            _sut = new AccountService(_accountRepository, _messageBus, _applicationRepository, _repository);
            _invitedAccount = new Account("arne", "1234");
            _invitedAccount.SetId(InvitedAccountId);
            _invitedAccount.SetVerifiedEmail("jonas@gauffin.com");
            _accountRepository.GetByIdAsync(InvitedAccountId).Returns(_invitedAccount);
        }

        private readonly IInvitationRepository _repository;
        private readonly IAccountRepository _accountRepository;
        private readonly AccountService _sut;
        private readonly IMessageBus _messageBus;
        private readonly Account _invitedAccount;
        private readonly IApplicationRepository _applicationRepository;
        private const int InvitedAccountId = 999;

        [Fact]
        public async Task
            Should_delete_invitation_when_its_accepted_to_prevent_creating_multiple_accounts_with_the_same_invitation_key()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request =
                new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            var principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.AcceptInvitation(principal, request);

            actual.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_notify_system_of_the_accepted_invitation()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request =
                new AcceptInvitation(InvitedAccountId, invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            var principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.AcceptInvitation(principal, request);

            await _messageBus.Received().SendAsync(principal, Arg.Any<InvitationAccepted>());
            var evt = _messageBus.Method("SendAsync").Arg<InvitationAccepted>();
            evt.AcceptedEmailAddress.Should().Be(request.AcceptedEmail);
            evt.AccountId.Should().Be(InvitedAccountId);
            evt.ApplicationIds[0].Should().Be(1);
            evt.UserName.Should().Be(_invitedAccount.UserName);
        }

        [Fact]
        public async Task Should_create_an_Account_for_invites_to_new_users()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request =
                new AcceptInvitation("arne", "pass", invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            var principal = PrincipalHelper.Create(52, "arne");


            var actual = await _sut.AcceptInvitation(principal, request);

            await _accountRepository.Received().CreateAsync(Arg.Any<Account>());
            var evt = _messageBus.Method("SendAsync").Arg<InvitationAccepted>();
            evt.AccountId.Should().Be(52);
        }

        [Fact]
        public async Task
            Should_publish_AccountRegistered_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request =
                new AcceptInvitation("arne", "pass", invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            var principal = PrincipalHelper.Create(52, "arne");


            var actual = await _sut.AcceptInvitation(principal, request);

            await _messageBus.Received().SendAsync(principal, Arg.Any<AccountRegistered>());
            var evt = _messageBus.Method("SendAsync").Arg<AccountRegistered>();
            evt.AccountId.Should().Be(52);
        }

        [Fact]
        public async Task
            Should_publish_AccountActivated_if_a_new_account_is_created_as_we_bypass_the_regular_account_registration_flow()
        {
            var invitation = new Invitation("invited@test.com", "inviter");
            var request =
                new AcceptInvitation("arne", "pass", invitation.InvitationKey) {AcceptedEmail = "arne@gauffin.com"};
            invitation.Add(1, "arne");
            _repository.GetByInvitationKeyAsync(request.InvitationKey).Returns(invitation);
            _accountRepository
                .WhenForAnyArgs(x => x.CreateAsync(null))
                .Do(x => x.Arg<Account>().SetId(52));
            var principal = PrincipalHelper.Create(52, "arne");


            var actual = await _sut.AcceptInvitation(principal, request);

            await _messageBus.Received().SendAsync(principal, Arg.Any<AccountActivated>());
            var evt = _messageBus.Method("SendAsync").Arg<AccountActivated>();
            evt.AccountId.Should().Be(52);
        }


        [Fact]
        public async Task Should_ignore_invitations_where_the_key_is_not_registered_in_the_db()
        {
            var request = new AcceptInvitation(InvitedAccountId, "invalid") {AcceptedEmail = "arne@gauffin.com"};
            var principal = PrincipalHelper.Create(52, "arne");

            var actual = await _sut.AcceptInvitation(principal, request);

            actual.Should().BeNull();
        }
    }
}