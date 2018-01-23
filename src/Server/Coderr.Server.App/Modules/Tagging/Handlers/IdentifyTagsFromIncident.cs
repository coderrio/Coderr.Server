using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents.Events;
using DotNetCqs;
using Griffin.Container;
using log4net;

namespace codeRR.Server.App.Modules.Tagging.Handlers
{
    /// <summary>
    ///     Scan through the error report to identify which libraries were used when the exception was thrown.
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class IdentifyTagsFromIncident : IMessageHandler<ReportAddedToIncident>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(IdentifyTagsFromIncident));
        private readonly ITagsRepository _repository;
        private readonly ITagIdentifierProvider _tagIdentifierProvider;

        /// <summary>
        ///     Creates a new instance of <see cref="IdentifyTagsFromIncident" />.
        /// </summary>
        /// <param name="repository">repository</param>
        /// <param name="tagIdentifierProvider">Used to be able to create tag identifiers in all modules</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public IdentifyTagsFromIncident(ITagsRepository repository, ITagIdentifierProvider tagIdentifierProvider)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _tagIdentifierProvider = tagIdentifierProvider;
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
            _logger.Debug("Checking tags..");
            var tags = await _repository.GetIncidentTagsAsync(e.Incident.Id);
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
                // Comma seperated tags
                if (collection.Properties.TryGetValue("OneTrueTags", out var tagsStr)
                    && collection.Properties.TryGetValue("ErrTags", out tagsStr))
                {
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

                //Tag array
                foreach (var property in collection.Properties)
                {
                    if (property.Key.StartsWith("ErrTags["))
                        ctx.AddTag(property.Value, 1);
                }
            }
        }
    }
}