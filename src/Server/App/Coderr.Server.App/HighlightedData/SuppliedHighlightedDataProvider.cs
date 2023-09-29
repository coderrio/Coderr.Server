using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;

namespace Coderr.Server.App.HighlightedData
{
    [ContainerService]
    internal class SuppliedHighlightedDataProvider : IHighlightedContextDataProvider
    {
        private readonly IQueryBus _queryBus;

        public SuppliedHighlightedDataProvider(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public async Task CollectAsync(HighlightedContextDataProviderContext context)
        {
            var query = new GetCollection(context.IncidentId, "CoderrData")
            {
                MaxNumberOfCollections = 1
            };
            var collection = await _queryBus.QueryAsync(query);
            if (collection.Items.Length == 0)
                return;


            var props = collection.Items[0]
                .Properties
                .Where(x => x.Key.StartsWith("Highlight."))
                .ToList();
            foreach (var pair in props)
            {
                var name = pair.Key.Remove(0, "Highlight.".Length);
                var data = new HighlightedContextData
                {
                    Description = "Data provided by the client library",
                    Name = name,
                    Value = new[] { pair.Value }
                };

                context.AddValue(data);

            }
        }
    }
}