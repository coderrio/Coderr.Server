using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Commands;
using DotNetCqs;

namespace Coderr.Server.App.Core.Environments.Handlers
{
    internal class CreateEnvironmentHandler : IMessageHandler<CreateEnvironment>
    {
        private readonly IEnvironmentRepository _repository;

        public CreateEnvironmentHandler(IEnvironmentRepository repository)
        {
            _repository = repository;
        }

        public async Task HandleAsync(IMessageContext context, CreateEnvironment message)
        {
            var env = await _repository.FindByName(message.Name);
            if (env == null)
            {
                env = new Environment(message.Name);
                await _repository.Create(env);
            }

            var ae = await _repository.Find(env.Id, message.ApplicationId);
            if (ae != null)
            {
                ae.DeleteIncidents = message.DeleteIncidents;
                await _repository.Update(ae);
                return;
            }

            ae = new ApplicationEnvironment(message.ApplicationId, env.Id) {DeleteIncidents = message.DeleteIncidents};
            await _repository.Create(ae);
        }
    }
}