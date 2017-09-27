using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Accounts.Events;
using codeRR.Api.Core.Applications.Events.codeRR.Api.Core.Accounts.Events;

namespace codeRR.App.Core.Applications.EventHandlers
{
    [Component(RegisterAsSelf = true)]
    internal class UpdateTeamOnInvitationAccepted : IApplicationEventSubscriber<InvitationAccepted>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IEventBus _eventBus;

        public UpdateTeamOnInvitationAccepted(IApplicationRepository applicationRepository, IEventBus eventBus)
        {
            _applicationRepository = applicationRepository;
            _eventBus = eventBus;
        }

        public async Task HandleAsync(InvitationAccepted e)
        {
            foreach (var applicationId in e.ApplicationIds)
            {
                var members = await _applicationRepository.GetTeamMembersAsync(applicationId);
                var member = members.FirstOrDefault(x => x.EmailAddress == e.InvitedEmailAddress && x.AccountId == 0);
                if (member != null)
                {
                    member.AcceptInvitation(e.AccountId);
                    await _applicationRepository.UpdateAsync(member);
                    await _eventBus.PublishAsync(new UserAddedToApplication(applicationId, e.AccountId));
                }
            }
        }
    }
}