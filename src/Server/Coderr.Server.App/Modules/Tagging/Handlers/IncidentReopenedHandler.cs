using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.App.Modules.Tagging.Domain;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Tagging.Handlers
{
    /// <summary>
    ///     Adds a "incident-reopened" tag
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class IncidentReopenedHandler : IMessageHandler<IncidentReOpened>
    {
        private readonly ITagsRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="IncidentReopenedHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public IncidentReopenedHandler(ITagsRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task HandleAsync(IMessageContext context, IncidentReOpened e)
        {
            var tags = await _repository.GetIncidentTagsAsync(e.IncidentId);
            if (tags.Any(x => x.Name == "incident-reopened"))
                return;

            await _repository.AddAsync(e.IncidentId, new[] {new Tag("incident-reopened", 1)});
        }
    }
}