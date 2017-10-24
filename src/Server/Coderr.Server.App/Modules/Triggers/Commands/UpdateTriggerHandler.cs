using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.Triggers.Commands;
using codeRR.Server.App.Modules.Triggers.Domain;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Triggers.Commands
{
    /// <summary>
    ///     Handler for <see cref="UpdateTrigger" />.
    /// </summary>
    [Component]
    public class UpdateTriggerHandler : IMessageHandler<UpdateTrigger>
    {
        private readonly ITriggerRepository _triggerRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateTriggerHandler" />.
        /// </summary>
        /// <param name="triggerRepository">repos</param>
        /// <exception cref="ArgumentNullException">triggerRepository</exception>
        public UpdateTriggerHandler(ITriggerRepository triggerRepository)
        {
            if (triggerRepository == null) throw new ArgumentNullException("triggerRepository");
            _triggerRepository = triggerRepository;
        }


        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, UpdateTrigger command)
        {
            if (command == null) throw new ArgumentNullException("command");
            var domainEntity = await _triggerRepository.GetAsync(command.Id);
            domainEntity.Description = command.Description;
            domainEntity.LastTriggerAction = DtoToDomainConverters.ConvertLastAction(command.LastTriggerAction);
            domainEntity.Name = command.Name;
            domainEntity.RunForExistingIncidents = command.RunForExistingIncidents;
            domainEntity.RunForReopenedIncidents = command.RunForReOpenedIncidents;
            domainEntity.RunForNewIncidents = command.RunForNewIncidents;

            domainEntity.RemoveActions();
            foreach (var action in command.Actions)
            {
                domainEntity.AddAction(DtoToDomainConverters.ConvertAction(action));
            }

            domainEntity.RemoveRules();
            foreach (var rule in command.Rules)
            {
                domainEntity.AddRule(DtoToDomainConverters.ConvertRule(rule));
            }

            await _triggerRepository.UpdateAsync(domainEntity);
        }
    }
}