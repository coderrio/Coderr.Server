using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.ErrorOrigins.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Geolocation
{
    /// <summary>
    ///     Handler for <see cref="GetOriginsForIncident" />.
    /// </summary>
    public class GetOriginsForIncidentHandler : IQueryHandler<GetOriginsForIncident, GetOriginsForIncidentResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;


        /// <summary>
        ///     Creates a new instance of <see cref="GetOriginsForIncidentHandler" />.
        /// </summary>
        /// <exception cref="ArgumentNullException">repository</exception>
        public GetOriginsForIncidentHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            var ourItems = await _unitOfWork.ToListAsync<ErrorOrginListItem>("IncidentId = @0", query.IncidentId);
            var items = from x in ourItems
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