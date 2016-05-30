using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Incidents.Events;
using OneTrueError.App.Modules.Tagging.Domain;

namespace OneTrueError.App.Modules.Tagging.Handlers
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

        /// <inheritdoc/>
        public async Task HandleAsync(IncidentReOpened e)
        {
            await _repository.AddAsync(e.IncidentId, new[] { new Tag("incident-reopened", 1) });
        }
    }
}