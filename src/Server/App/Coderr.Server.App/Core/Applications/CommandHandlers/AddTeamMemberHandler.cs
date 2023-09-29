using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    public class AddTeamMemberHandler : IMessageHandler<AddTeamMember>
    {
        private readonly IApplicationRepository _applicationRepository;

        public AddTeamMemberHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task HandleAsync(IMessageContext context, AddTeamMember message)
        {
            var member = new ApplicationTeamMember(message.ApplicationId, message.UserToAdd,
                context.Principal.Identity.Name);

            await _applicationRepository.CreateAsync(member);
        }
    }
}