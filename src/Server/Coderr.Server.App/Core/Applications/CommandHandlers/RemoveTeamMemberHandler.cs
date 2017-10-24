using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Remove a team member from an application
    /// </summary>
    [Component]
    public class RemoveTeamMemberHandler : IMessageHandler<RemoveTeamMember>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RemoveTeamMember));

        /// <summary>
        ///     Creates a new instance of <see cref="RemoveTeamMemberHandler" />.
        /// </summary>
        /// <param name="applicationRepository">To remove member</param>
        public RemoveTeamMemberHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        /// <inheritdoc />
        public async Task HandleAsync(IMessageContext context, RemoveTeamMember command)
        {
            context.Principal.EnsureApplicationAdmin(command.ApplicationId);
            await _applicationRepository.RemoveTeamMemberAsync(command.ApplicationId, command.UserToRemove);
            _logger.Info("Removed " + command.UserToRemove + " from application " + command.ApplicationId);
        }
    }
}