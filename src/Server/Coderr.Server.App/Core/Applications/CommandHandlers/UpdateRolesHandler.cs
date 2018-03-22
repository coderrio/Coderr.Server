using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    public class UpdateRolesHandler : IMessageHandler<UpdateRoles>
    {
        private readonly IApplicationRepository _applicationRepository;

        public UpdateRolesHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task HandleAsync(IMessageContext context, UpdateRoles message)
        {
            var apps = await _applicationRepository.GetTeamMembersAsync(message.ApplicationId);
            var user = apps.FirstOrDefault(x => x.AccountId == message.UserToUpdate);
            user.Roles = message.Roles;
            await _applicationRepository.UpdateAsync(user);
        }
    }
}