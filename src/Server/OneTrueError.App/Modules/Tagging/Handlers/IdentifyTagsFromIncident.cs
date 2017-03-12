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
        private readonly ILog _logger = LogManager.GetLogger(typeof(IdentifyTagsFromIncident));
        private readonly ITagsRepository _repository;
        private ITagIdentifierProvider _tagIdentifierProvider;

        /// <summary>
        ///     Creates a new instance of <see cref="IdentifyTagsFromIncident" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="tagIdentifierProvider">Used to be able to create tag identifiers in all modules</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public IdentifyTagsFromIncident(ITagsRepository repository, ITagIdentifierProvider tagIdentifierProvider)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
            _tagIdentifierProvider = tagIdentifierProvider;
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
            _logger.Debug("Checking tags..");
            var tags = await _repository.GetTagsAsync(e.Incident.Id);
            var ctx = new TagIdentifierContext(e.Report, tags);
            var identifiers = _tagIdentifierProvider.GetIdentifiers(ctx);
            foreach (var identifier in identifiers)
            {
                identifier.Identify(ctx);
            }

            ExtractTagsFromCollections(e, ctx);

            _logger.DebugFormat("Done, identified {0} new tags", ctx.NewTags);

            if (ctx.NewTags.Count == 0)
                return;

            await _repository.AddAsync(e.Incident.Id, ctx.NewTags.ToArray());
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