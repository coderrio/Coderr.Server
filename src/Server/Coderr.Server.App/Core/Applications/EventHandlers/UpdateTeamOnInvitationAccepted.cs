using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Events;
using codeRR.Server.Api.Core.Applications.Events;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Applications.EventHandlers
{
    [Component(RegisterAsSelf = true)]
    internal class UpdateTeamOnInvitationAccepted : IMessageHandler<InvitationAccepted>
    {
        private readonly IApplicationRepository _applicationRepository;

        public UpdateTeamOnInvitationAccepted(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task HandleAsync(IMessageContext context, InvitationAccepted e)
        {
            foreach (var applicationId in e.ApplicationIds)
            {
                var members = await _applicationRepository.GetTeamMembersAsync(applicationId);
                var member = members.FirstOrDefault(x => x.EmailAddress == e.InvitedEmailAddress && x.AccountId == 0);
                if (member != null)
                {
                    member.AcceptInvitation(e.AccountId);
                    await _applicationRepository.UpdateAsync(member);
                    await context.SendAsync(new UserAddedToApplication(applicationId, e.AccountId));
                }
            }
        }
    }
}