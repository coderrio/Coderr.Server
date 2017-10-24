using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using codeRR.Server.App.Modules.Triggers.Domain;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Modules.Triggers.EventHandlers
{
    /// <summary>
    ///     Responsible of creating context collection metadata for all reports that have been added to an incident.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class UpdateCollectionsOnReportAdded : IMessageHandler<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(UpdateCollectionsOnReportAdded));
        private readonly ITriggerRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdateCollectionsOnReportAdded" />.
        /// </summary>
        /// <param name="repository">repository</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public UpdateCollectionsOnReportAdded(ITriggerRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        ///     Process an event asynchronously.
        /// </summary>
        /// <param name="e">event to process</param>
        /// <returns>
        ///     Task to wait on.
        /// </returns>
        public async Task HandleAsync(IMessageContext context, ReportAddedToIncident e)
        {
            if (e == null) throw new ArgumentNullException("e");

            _logger.Debug("doing collections");
            var collections = await _repository.GetCollectionsAsync(e.Incident.ApplicationId);
            foreach (var collectionDto in e.Report.ContextCollections)
            {
                var isNew = false;
                var meta =
                    collections.FirstOrDefault(x => x.Name.Equals(collectionDto.Name, StringComparison.OrdinalIgnoreCase));
                if (meta == null)
                {
                    isNew = true;
                    meta = new CollectionMetadata(e.Incident.ApplicationId, collectionDto.Name);
                }

                foreach (var property in collectionDto.Properties)
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