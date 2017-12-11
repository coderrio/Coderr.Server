using System;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Applications.Events;
using codeRR.Server.Api.Core.Invitations.Commands;
using codeRR.Server.Api.Core.Messaging;
using codeRR.Server.Api.Core.Messaging.Commands;
using codeRR.Server.App.Configuration;
using codeRR.Server.App.Core.Applications;
using codeRR.Server.App.Core.Users;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Security;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using DotNetCqs.DependencyInjection;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Core.Invitations.CommandHandlers
{
    /// <summary>
    ///     Handler for <see cref="InviteUser" />
    /// </summary>
    /// <remarks>
    ///     <para>
    /// 
    ///     </para>
    /// </remarks>
    [Component]
    public class InviteUserHandler : IMessageHandler<InviteUser>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(InviteUserHandler));
        private ConfigurationStore _configStore;

        /// <summary>
        ///     Creates a new instance of <see cref="InviteUserHandler" />.
        /// </summary>
        /// <param name="invitationRepository">Store invitations</param>
        /// <param name="userRepository">To load inviter and invitee</param>
        /// <param name="applicationRepository">Add pending member</param>
        public InviteUserHandler(IInvitationRepository invitationRepository,
            IUserRepository userRepository, IApplicationRepository applicationRepository, ConfigurationStore configStore)
        {
            _invitationRepository = invitationRepository;
            _userRepository = userRepository;
            _applicationRepository = applicationRepository;
            _configStore = configStore;
        }

        /// <inheritdoc />
        public async Task HandleAsync(IMessageContext context, InviteUser command)
        {
            var inviter = await _userRepository.GetUserAsync(command.UserId);
            if (!context.Principal.IsSysAdmin() &&
                !context.Principal.IsApplicationAdmin(command.ApplicationId))
            {
                _logger.Warn($"User {command.UserId} attempted to do an invite for an application: {command.ApplicationId}.");
                throw new SecurityException("You are not an admin of that application.");
            }

            var invitedUser = await _userRepository.FindByEmailAsync(command.EmailAddress);
            if (invitedUser != null)
            {
                //correction of issue #21, verify that the person isn't already a member.
                var members = await _applicationRepository.GetTeamMembersAsync(command.ApplicationId);
                if (members.Any(x => x.AccountId == invitedUser.AccountId))
                {
                    _logger.Warn("User " + invitedUser.AccountId + " is already a member.");
                    return;
                }

                var member = new ApplicationTeamMember(command.ApplicationId, invitedUser.AccountId, inviter.UserName)
                {
                    Roles = new[] {ApplicationRole.Member}
                };

                await _applicationRepository.CreateAsync(member);
                await context.SendAsync(new UserAddedToApplication(command.ApplicationId, member.AccountId));
                return;
            }
            else
            {
                //correction of issue #21, verify that the person isn't already a member.
                var members = await _applicationRepository.GetTeamMembersAsync(command.ApplicationId);
                if (members.Any(x => x.EmailAddress == command.EmailAddress))
                {
                    _logger.Warn("User " + command.EmailAddress + " is already invited.");
                    return;
                }
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
                await SendInvitationEmailAsync(context, invitation, command.Text);
            }

            invitation.Add(command.ApplicationId, inviter.UserName);
            await _invitationRepository.UpdateAsync(invitation);

            var app = await _applicationRepository.GetByIdAsync(command.ApplicationId);
            var evt = new UserInvitedToApplication(
                invitation.InvitationKey,
                command.ApplicationId,
                app.Name,
                command.EmailAddress,
                inviter.UserName);

            await context.SendAsync(evt);
        }

        /// <summary>
        /// Send invitation email
        /// </summary>
        /// <param name="invitation">Invitation to generate an email for</param>
        /// <param name="reason">Why the user was invited (optional)</param>
        /// <returns>task</returns>
        protected virtual async Task SendInvitationEmailAsync(IMessageContext context, Invitation invitation, string reason)
        {
            var config = _configStore.Load<BaseConfiguration>();
            var url = config.BaseUrl.ToString().TrimEnd('/');
            if (string.IsNullOrEmpty(reason))
                reason = "";
            else
                reason += "\r\n";

            var msg = new EmailMessage
            {
                Subject = "You have been invited by " + invitation.InvitedBy + " to codeRR.",
                TextBody = string.Format(@"Hello,

{0} has invited to you join their team at codeRR, a service used to keep track of exceptions in .NET applications.

Click on the following link to accept the invitation:
{2}/account/accept/{1}

{3}
Best regards,
  The codeRR team
", invitation.InvitedBy, invitation.InvitationKey, url, reason),
                Recipients = new[] {new EmailAddress(invitation.EmailToInvitedUser)}
            };

            await context.SendAsync(new SendEmail(msg));
        }
    }
}