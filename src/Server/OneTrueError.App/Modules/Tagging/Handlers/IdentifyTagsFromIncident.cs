using System;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using OneTrueError.Api.Core.Incidents.Events;

namespace OneTrueError.App.Modules.Tagging.Handlers
{
    /// <summary>
    ///     Scan through the error report to identify which libraries were used when the exception was thrown.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class IdentifyTagsFromIncident : IApplicationEventSubscriber<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof (IdentifyTagsFromIncident));
        private readonly ITagsRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="IdentifyTagsFromIncident" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public IdentifyTagsFromIncident(ITagsRepository repository)
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
            if (e.Incident.ReportCount != 1)
            {
                var tags = await _repository.GetTagsAsync(e.Incident.Id);
                if (tags.Count > 0)
                    return;
            }

            _logger.Debug("Checking tags..");
            var ctx = new TagIdentifierContext(e.Report);
            var identifierProvider = new IdentifierProvider();
            var identifiers = identifierProvider.GetIdentifiers(ctx);
            foreach (var identifier in identifiers)
            {
                identifier.Identify(ctx);
            }

            ExtractTagsFromCollections(e, ctx);

            _logger.Debug("done..");

            await _repository.AddAsync(e.Incident.Id, ctx.Tags.ToArray());
        }

        private void ExtractTagsFromCollections(ReportAddedToIncident e, TagIdentifierContext ctx)
        {
            foreach (var collection in e.Report.ContextCollections)
            {
                string tagsStr;
                if (!collection.Properties.TryGetValue("OneTrueTags", out tagsStr))
                    continue;

                try
                {
                    var tags = tagsStr.Split(',');
                    foreach (var tag in tags)
                    {
                        ctx.AddTag(tag, 1);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(
                        "Failed to parse tags from '" + collection.Name + "', invalid tag string: '" + tagsStr + "'.",
                        ex);
                }
            }
        }
    }
}