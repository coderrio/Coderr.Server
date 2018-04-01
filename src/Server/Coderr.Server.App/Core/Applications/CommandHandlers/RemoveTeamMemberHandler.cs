using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using log4net;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    /// <summary>
    ///     Remove a team member from an application
    /// </summary>
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