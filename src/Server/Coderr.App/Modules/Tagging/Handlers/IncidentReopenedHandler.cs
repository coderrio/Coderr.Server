using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Incidents.Events;
using codeRR.App.Modules.Tagging.Domain;

namespace codeRR.App.Modules.Tagging.Handlers
{
    /// <summary>
    ///     Adds a "incident-reopened" tag
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class IncidentReopenedHandler : IApplicationEventSubscriber<IncidentReOpened>
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
        public async Task HandleAsync(IncidentReOpened e)
        {
            var tags = await _repository.GetTagsAsync(e.IncidentId);
            if (tags.Any(x => x.Name == "incident-reopened"))
                return;

            await _repository.AddAsync(e.IncidentId, new[] {new Tag("incident-reopened", 1)});
        }
    }
}