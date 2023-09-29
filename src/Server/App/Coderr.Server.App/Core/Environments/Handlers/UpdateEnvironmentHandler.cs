using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Api.Core.Environments.Commands;
using DotNetCqs;

namespace Coderr.Server.App.Core.Environments.Handlers
{
    internal class UpdateEnvironmentHandler : IMessageHandler<UpdateEnvironment>
    {
        private readonly IEnvironmentRepository _repository;

        public UpdateEnvironmentHandler(IEnvironmentRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, UpdateEnvironment message)
        {
            var appEnv = await _repository.Find(message.EnvironmentId, message.ApplicationId);
            if (appEnv == null)
            {
                var env = await _repository.Find(message.EnvironmentId);
                if (env == null)
                {
                    Err.ReportLogicError("Failed to find environment " + message.EnvironmentId,
                        new
                        {
                            UserId = context.Principal.Identity.Name
                        });
                    return;
                }

                appEnv = new ApplicationEnvironment(message.ApplicationId, message.EnvironmentId)
                {
                    Name = env.Name,
                    DeleteIncidents = message.DeleteIncidents
                };
            }
            else
            {
                appEnv.DeleteIncidents = message.DeleteIncidents;
            }

            await _repository.Update(appEnv);
        }
    }
}