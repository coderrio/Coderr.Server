using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.Triggers.Queries;
using codeRR.Server.App.Modules.Triggers.Domain;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Triggers.Queries
{
    /// <summary>
    ///     Handler for <see cref="GetTrigger" />.
    /// </summary>
    [Component]
    public class GetTriggerHandler : IQueryHandler<GetTrigger, GetTriggerDTO>
    {
        private readonly ITriggerRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetTriggerHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public GetTriggerHandler(ITriggerRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<GetTriggerDTO> HandleAsync(IMessageContext context, GetTrigger query)
        {
            var trigger = await _repository.GetAsync(query.Id);
            return new GetTriggerDTO
            {
                ApplicationId = trigger.ApplicationId,
                Actions = trigger.Actions.Select(DomainToDtoConverters.ConvertAction).ToArray(),
                Description = trigger.Description,
                LastTriggerAction = DomainToDtoConverters.ConvertLastAction(trigger.LastTriggerAction),
                Id = trigger.Id,
                Name = trigger.Name,
                Rules = trigger.Rules.Select(DomainToDtoConverters.ConvertRule).ToArray(),
                RunForExistingIncidents = trigger.RunForExistingIncidents,
                RunForReOpenedIncidents = trigger.RunForReopenedIncidents,
                RunForNewIncidents = trigger.RunForNewIncidents
            };
        }
    }
}