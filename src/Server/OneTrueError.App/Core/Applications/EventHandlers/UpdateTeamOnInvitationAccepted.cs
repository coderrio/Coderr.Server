using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Events;

namespace OneTrueError.App.Core.Applications.EventHandlers
{
    [Component(RegisterAsSelf = true)]
    internal class UpdateTeamOnInvitationAccepted : IApplicationEventSubscriber<InvitationAccepted>
    {
        private readonly IApplicationRepository _applicationRepository;

        public UpdateTeamOnInvitationAccepted(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task HandleAsync(InvitationAccepted e)
        {
            foreach (var applicationId in e.ApplicationIds)
            {
                var members = await _applicationRepository.GetTeamMembersAsync(applicationId);
                var member = members.FirstOrDefault(x => x.EmailAddress == e.EmailAddress && x.AccountId == 0);
                if (member != null)
                {
                    member.AccountId = e.AccountId;
                    await _applicationRepository.UpdateAsync(member);
                }
            }
        }
    }
}