using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using codeRR.Api.Core.Incidents.Events;
using codeRR.App.Modules.Triggers.Domain;

namespace codeRR.App.Modules.Triggers.EventHandlers
{
    /// <summary>
    ///     Responsible of creating context collection metadata for all reports that have been added to an incident.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class UpdateCollectionsOnReportAdded : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UpdateCollectionsOnReportAdded));
        private readonly ITriggerRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateCollectionsOnReportAdded" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public UpdateCollectionsOnReportAdded(ITriggerRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(ReportAddedToIncident e)
        {
            if (e == null) throw new ArgumentNullException("e");

            _logger.Debug("doing collections");
            var collections = await _repository.GetCollectionsAsync(e.Incident.ApplicationId);
            foreach (var context in e.Report.ContextCollections)
            {
                var isNew = false;
                var meta =
                    collections.FirstOrDefault(x => x.Name.Equals(context.Name, StringComparison.OrdinalIgnoreCase));
                if (meta == null)
                {
                    isNew = true;
                    meta = new CollectionMetadata(e.Incident.ApplicationId, context.Name);
                }

                foreach (var property in context.Properties)
                {
                    meta.AddOrUpdateProperty(property.Key);
                }

                if (!meta.IsUpdated)
                    continue;

                if (isNew)
                    await _repository.CreateAsync(meta);
                else
                    await _repository.UpdateAsync(meta);
            }

            _logger.Debug("collections done");
        }
    }
}