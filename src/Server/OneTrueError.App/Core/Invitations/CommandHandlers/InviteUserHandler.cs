using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Applications.Events;
using OneTrueError.Api.Core.Invitations.Commands;
using OneTrueError.Api.Core.Messaging;
using OneTrueError.Api.Core.Messaging.Commands;
using OneTrueError.App.Configuration;
using OneTrueError.App.Core.Applications;
using OneTrueError.App.Core.Users;
using OneTrueError.Infrastructure.Configuration;

namespace OneTrueError.App.Core.Invitations.CommandHandlers
{
    [Component]
    internal class InviteUserHandler : ICommandHandler<InviteUser>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICommandBus _commandBus;
        private readonly IEventBus _eventBus;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserRepository _userRepository;

        public InviteUserHandler(IInvitationRepository invitationRepository, IEventBus eventBus,
            IUserRepository userRepository, IApplicationRepository applicationRepository, ICommandBus commandBus)
        {
            _invitationRepository = invitationRepository;
            _eventBus = eventBus;
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
            _commandBus = commandBus;
        }

        public async Task ExecuteAsync(InviteUser command)
        {
            var inviter = await _userRepository.GetUserAsync(command.UserId);
            var invitedUser = await _userRepository.FindByEmailAsync(command.EmailAddress);
            if (invitedUser != null)
            {
                var member = new ApplicationTeamMember(command.ApplicationId, command.EmailAddress)
                {
                    AccountId = invitedUser.AccountId,
                    AddedByName = inviter.UserName,
                    Roles = new[] {ApplicationRole.Member}
                };

                await _applicationRepository.CreateAsync(member);
                return;
            }

            var invitedMember = new ApplicationTeamMember(command.ApplicationId, command.EmailAddress)
            {
                AddedByName = inviter.UserName,
                Roles = new[] {ApplicationRole.Member}
            };
            await _applicationRepository.CreateAsync(invitedMember);

            var invitation = await _invitationRepository.FindByEmailAsync(command.EmailAddress);
            if (invitation == null)
            {
                invitation = new Invitation(command.EmailAddress, inviter.UserName);
                await _invitationRepository.CreateAsync(invitation);
                await SendInvitationEmailAsync(invitation, command.Text);
            }

            invitation.Add(command.ApplicationId, inviter.UserName);
            try
            {
                await _invitationRepository.UpdateAsync(invitation);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            var evt = new UserInvitedToApplication(command.ApplicationId,
                command.EmailAddress,
                inviter.UserName);

            await _eventBus.PublishAsync(evt);
        }

        protected virtual async Task SendInvitationEmailAsync(Invitation invitation, string reason)
        {
            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            var url = config.BaseUrl.ToString().TrimEnd('/');
            if (string.IsNullOrEmpty(reason))
                reason = "";
            else
                reason += "\r\n";

            var msg = new EmailMessage
            {
                Subject = "You have been invited by " + invitation.InvitedBy + " to OneTrueError.",
                TextBody = string.Format(@"Hello,

{0} has invited to you join their team at OneTrueError, a service used to keep track of exceptions in .NET applications.

Click on the following link to accept the invitation:
{2}/account/accept/{1}

{3}
Best regards,
  The OneTrueError team
", invitation.InvitedBy, invitation.InvitationKey, url, reason),
                Recipients = new[] {new EmailAddress(invitation.EmailToInvitedUser)}
            };

            await _commandBus.ExecuteAsync(new SendEmail(msg));
        }
    }
}