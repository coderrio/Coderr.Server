using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Abstractions.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Api.Core.Reports.Queries;
using DotNetCqs;

namespace Coderr.Server.App.HighlightedData
{
    [ContainerService]
    internal class RequestProvider : IHighlightedContextDataProvider
    {
        private readonly IQueryBus _queryBus;

        public RequestProvider(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        public async Task CollectAsync(HighlightedContextDataProviderContext context)
        {
            if (!context.Tags.Contains("http"))
                return;

            var query = new GetReportList(context.IncidentId)
            {
                PageNumber = 1,
                PageSize = 1
            };
            var reports = await _queryBus.QueryAsync(query);

            var query2 = new GetReport(reports.Items[0].Id);
            var report = await _queryBus.QueryAsync(query2);

            var request = report.ContextCollections.FirstOrDefault(x => x.Name == "HttpRequest");
            var formData = report.ContextCollections.FirstOrDefault(x => x.Name == "HttpForm");
            if (formData == null && request == null)
                return;


            if (formData != null)
            {
                foreach (var property in formData.Properties)
                {
                    context.AddValue(new HighlightedContextData
                    {
                        Name = property.Key,
                        Description = "HTTP Form field",
                        Value = new[] {property.Value}
                    });
                }
            }

            if (request != null)
            {
                var url = request.Properties.FirstOrDefault(x => x.Key == "Url" || x.Key == "Uri");
                if (url != null)
                {
                    context.AddValue(new HighlightedContextData
                    {
                        Description = "Visted URL",
                        Name = "Url",
                        Value = new[] {url.Value}
                    });
                }

                var referrer = request.Properties.FirstOrDefault(x =>
                    x.Key == "Referrer" || x.Key == "HttpReferrer" || x.Key == "Referer" || x.Key == "HttpReferer");
                if (referrer != null)
                {
                    context.AddValue(new HighlightedContextData
                    {
                        Description = "Which page the user came from",
                        Name = "Referrer",
                        Value = new[] {referrer.Value}
                    });
                }
            }
        }
    }
}