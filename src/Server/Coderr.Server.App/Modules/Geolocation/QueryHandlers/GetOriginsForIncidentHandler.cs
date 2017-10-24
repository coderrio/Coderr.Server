using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.ErrorOrigins.Queries;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Geolocation.QueryHandlers
{
    /// <summary>
    ///     Handler for <see cref="GetOriginsForIncident" />.
    /// </summary>
    [Component]
    public class GetOriginsForIncidentHandler : IQueryHandler<GetOriginsForIncident, GetOriginsForIncidentResult>
    {
        private readonly IErrorOriginRepository _repository;


        /// <summary>
        ///     Creates a new instance of <see cref="GetOriginsForIncidentHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public GetOriginsForIncidentHandler(IErrorOriginRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<GetOriginsForIncidentResult> HandleAsync(IMessageContext context, GetOriginsForIncident query)
        {
            var reports = await _repository.FindForIncidentAsync(query.IncidentId);
            var items = from x in reports
                select new GetOriginsForIncidentResultItem
                {
                    Longitude = x.Longitude,
                    Latitude = x.Latitude,
                    NumberOfErrorReports = x.NumberOfErrorReports
                };
            return new GetOriginsForIncidentResult {Items = items.ToArray()};
        }
    }
}