using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.Triggers.Commands;
using codeRR.Server.App.Modules.Triggers.Domain;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Triggers.Commands
{
    /// <summary>
    ///     Handler for <see cref="CreateTrigger" />.
    /// </summary>
    [Component]
    public class CreateTriggerHandler : IMessageHandler<CreateTrigger>
    {
        private readonly ITriggerRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="CreateTriggerHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        public CreateTriggerHandler(ITriggerRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }


        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, CreateTrigger command)
        {
            var domainModel = new Trigger(command.ApplicationId)
            {
                Description = command.Description,
                LastTriggerAction = DtoToDomainConverters.ConvertLastAction(command.LastTriggerAction),
                Id = command.Id,
                Name = command.Name,
                RunForExistingIncidents = command.RunForExistingIncidents,
                RunForReopenedIncidents = command.RunForReOpenedIncidents,
                RunForNewIncidents = command.RunForNewIncidents
            };

            foreach (var action in command.Actions)
            {
                domainModel.AddAction(DtoToDomainConverters.ConvertAction(action));
            }
            foreach (var rule in command.Rules)
            {
                domainModel.AddRule(DtoToDomainConverters.ConvertRule(rule));
            }

            await _repository.CreateAsync(domainModel);
        }
    }
}