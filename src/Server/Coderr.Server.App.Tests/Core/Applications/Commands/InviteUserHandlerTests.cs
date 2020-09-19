using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Applications.Events;
using Coderr.Server.Api.Core.Invitations.Commands;
using Coderr.Server.Api.Core.Messaging.Commands;
using Coderr.Server.App.Core.Invitations;
using Coderr.Server.App.Core.Invitations.CommandHandlers;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Coderr.Server.App.Tests.Core.Applications.Commands
{
    public class InviteUserHandlerTests
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly InviteUserHandler _sut;
        private readonly IUserRepository _userRepository;
        private readonly IMessageContext _context;
        private readonly TestStore _configStore;

        public InviteUserHandlerTests()
        {
            _invitationRepository = Substitute.For<IInvitationRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _applicationRepository = Substitute.For<IApplicationRepository>();
            _userRepository.GetUserAsync(1).Returns(new User(1, "First"));
            _applicationRepository.GetByIdAsync(1).Returns(new Application(1, "MyApp"));
            _context = Substitute.For<IMessageContext>();
            _configStore = new TestStore();
            _sut = new InviteUserHandler(_invitationRepository, _userRepository, _applicationRepository, new ConfigWrapper<BaseConfiguration>(_configStore));
        }

        [Fact]
        public async Task Should_create_an_invite_for_a_new_user()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(1, 3, "karl") };
            ApplicationTeamMember actual = null;
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _applicationRepository.WhenForAnyArgs(x => x.CreateAsync(Arg.Any<ApplicationTeamMember>()))
                .Do(x => actual = x.Arg<ApplicationTeamMember>());
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _applicationRepository.Received().CreateAsync(Arg.Any<ApplicationTeamMember>());
            actual.EmailAddress.Should().Be(cmd.EmailAddress);
            actual.ApplicationId.Should().Be(cmd.ApplicationId);
            actual.AddedAtUtc.Should().BeCloseTo(DateTime.UtcNow, 1000);
            actual.AddedByName.Should().Be("First");
        }

        [Fact]
        public void Regular_user_should_not_be_able_to_invite()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(1, 3, "karl") };
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _context.Principal.Returns(CreateUserPrincipal());

            Func<Task> actual = async () => await _sut.HandleAsync(_context, cmd);

            actual.Should().Throw<SecurityException>();
        }

        [Fact]
        public async Task Sysadmin_should_be_able_To_invite_users()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(3, 3, "karl") };
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _context.Received().SendAsync(Arg.Any<UserInvitedToApplication>());
        }

        [Fact]
        public async Task Should_not_allow_invites_when_the_invited_user_already_have_an_account()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 2 };
            var members = new[] { new ApplicationTeamMember(1, 3, "karl") };
            _userRepository.FindByEmailAsync(cmd.EmailAddress).Returns(new User(3, "existing"));
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _applicationRepository.DidNotReceive().CreateAsync(Arg.Any<ApplicationTeamMember>());
        }

        [Fact]
        public async Task Should_not_allow_invites_when_the_invited_user_already_have_an_pending_invite()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(1, cmd.EmailAddress) };
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _applicationRepository.DidNotReceive().CreateAsync(Arg.Any<ApplicationTeamMember>());
        }

        [Fact]
        public async Task Should_notify_the_system_of_the_invite()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(1, 3, "karl") };
            ApplicationTeamMember actual = null;
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _applicationRepository.WhenForAnyArgs(x => x.CreateAsync(Arg.Any<ApplicationTeamMember>()))
                .Do(x => actual = x.Arg<ApplicationTeamMember>());
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _applicationRepository.Received().CreateAsync(Arg.Any<ApplicationTeamMember>());
            await _context.Received().SendAsync(Arg.Any<UserInvitedToApplication>());
        }

        [Fact]
        public async Task Should_send_an_invitation_email()
        {
            var cmd = new InviteUser(1, "jonas@gauffin.com") { UserId = 1 };
            var members = new[] { new ApplicationTeamMember(1, 3, "karl") };
            ApplicationTeamMember actual = null;
            _applicationRepository.GetTeamMembersAsync(1).Returns(members);
            _applicationRepository.WhenForAnyArgs(x => x.CreateAsync(Arg.Any<ApplicationTeamMember>()))
                .Do(x => actual = x.Arg<ApplicationTeamMember>());
            _context.Principal.Returns(CreateAdminPrincipal());

            await _sut.HandleAsync(_context, cmd);

            await _context.Received().SendAsync(Arg.Any<SendEmail>());
        }

        private ClaimsPrincipal CreateAdminPrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, CoderrRoles.SysAdmin),
                new Claim(CoderrClaims.ApplicationAdmin, "1")
            };
            var identity = new ClaimsIdentity(claims);
            return new ClaimsPrincipal(identity);
        }

        private ClaimsPrincipal CreateUserPrincipal()
        {
            var claims = new List<Claim>
            {
            };
            var identity = new ClaimsIdentity(claims, AuthenticationTypes.Default);
            return new ClaimsPrincipal(identity);
        }
    }
}