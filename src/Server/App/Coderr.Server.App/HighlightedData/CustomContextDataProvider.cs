using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;

namespace Coderr.Server.App.HighlightedData
{
    [ContainerService]
    internal class CustomContextDataProvider : IHighlightedContextDataProvider
    {
        private readonly IQueryBus _queryBus;

        public CustomContextDataProvider(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public async Task CollectAsync(HighlightedContextDataProviderContext context)
        {
            var query = new GetCollection(context.IncidentId, "ContextData")
            {
                MaxNumberOfCollections = 1
            };
            var collection = await _queryBus.QueryAsync(query);
            if (collection.Items.Length == 0)
                return;


            string[] values;
            var name = "ContextData";
            var props = collection.Items[0].Properties;
            if (props.Count > 1)
            {
                values = props
                    .Select(x => $"{x.Key}: {x.Value}")
                    .ToArray();
            }
            else
            {
                var first = props.First();
                name = first.Key;
                values = new[] { first.Value };
            }

            var data = new HighlightedContextData
            {
                Description = "Data supplied by the developer",
                Name = name,
                Value = values
            };

            context.AddValue(data);
        }
    }
}