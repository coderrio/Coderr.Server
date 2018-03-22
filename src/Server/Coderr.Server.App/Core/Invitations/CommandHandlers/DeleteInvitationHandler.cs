using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Invitations.Commands;
using Coderr.Server.Api.Core.Invitations.Events;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;

namespace Coderr.Server.App.Core.Invitations.CommandHandlers
{
    internal class DeleteInvitationHandler : IMessageHandler<DeleteInvitation>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IInvitationRepository _invitationRepository;

        public DeleteInvitationHandler(IApplicationRepository applicationRepository,
            IInvitationRepository invitationRepository)
        {
            _applicationRepository = applicationRepository;
            _invitationRepository = invitationRepository;
        }

        public async Task HandleAsync(IMessageContext context, DeleteInvitation message)
        {
            var invite = await _invitationRepository.FindByEmailAsync(message.InvitedEmailAddress);
            await _applicationRepository.RemoveTeamMemberAsync(message.ApplicationId, message.InvitedEmailAddress);

            invite.Remove(message.ApplicationId);
            if (!invite.Invitations.Any())
            {
                await _invitationRepository.DeleteAsync(invite.InvitationKey);
                await context.SendAsync(new InvitationDeleted
                {
                    ApplicationIds = new[] {message.ApplicationId},
                    InvitedEmailAddress = message.InvitedEmailAddress,
                    InvitationId = invite.Id
                });
            }

            else
            {
                await _invitationRepository.UpdateAsync(invite);
            }
        }
    }
}