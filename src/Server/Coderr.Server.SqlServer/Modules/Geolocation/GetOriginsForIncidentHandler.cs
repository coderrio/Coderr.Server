using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.ErrorOrigins.Queries;
using DotNetCqs;
using Griffin.Data;

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
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"SELECT Longitude, Latitude, count(*) 
                                    FROM ErrorOrigins eo
                                    JOIN ErrorReportOrigins ON (eo.Id = ErrorReportOrigins.ErrorOriginId)
                                    WHERE IncidentId = @id
                                    GROUP BY IncidentId, Longitude, Latitude";
                cmd.AddParameter("id", query.IncidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<GetOriginsForIncidentResultItem>();
                    while (await reader.ReadAsync())
                    {
                        var item = new GetOriginsForIncidentResultItem
                        {
                            Longitude = (double) reader.GetDecimal(0),
                            Latitude = (double) reader.GetDecimal(1),
                            NumberOfErrorReports = reader.GetInt32(2)
                        };
                        items.Add(item);
                    }

                    return new GetOriginsForIncidentResult {Items = items.ToArray()};
                    ;
                }
            }
        }
    }
}