using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Commands;
using DotNetCqs;
using log4net;

namespace Coderr.Server.App.Core.Environments.Handlers
{
    internal class ResetEnvironmentHandler : IMessageHandler<ResetEnvironment>
    {
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly ILog _loggr = LogManager.GetLogger(typeof(ResetEnvironmentHandler));

        public ResetEnvironmentHandler(IEnvironmentRepository environmentRepository)
        {
            _environmentRepository = environmentRepository;
        }

        public async Task HandleAsync(IMessageContext context, ResetEnvironment message)
        {
            _loggr.Info("Resetting environmentId " + message.EnvironmentId + " for app " + message.ApplicationId);
            await _environmentRepository.Reset(message.EnvironmentId,
                message.ApplicationId == 0 ? (int?)null : message.ApplicationId);
        }
    }
}