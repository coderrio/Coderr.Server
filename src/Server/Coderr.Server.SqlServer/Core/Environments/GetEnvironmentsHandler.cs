using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Environments
{
    public class GetEnvironmentsHandler : IQueryHandler<GetEnvironments, GetEnvironmentsResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetEnvironmentsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetEnvironmentsResult> HandleAsync(IMessageContext context, GetEnvironments query)
        {
            var result = new GetEnvironmentsResult();

            string sql;
            if (query.ApplicationId == null)
            {
                sql = @"select Id, Name from Environments ORDER BY Name";
            }
            else
            {
                sql = @"WITH EnvironmentIds
                            AS
                            (
	                            select distinct EnvironmentId 
	                            FROM IncidentEnvironments
	                            JOIN Incidents ON (IncidentId = Incidents.Id)
	                            WHERE ApplicationId = @applicationId
                            )
                            SELECT Id, Name
                            FROM Environments 
                            JOIN EnvironmentIds ON (EnvironmentId=EnvironMents.Id)";
            }

            var items = await _unitOfWork
                .ToListAsync<GetEnvironmentsResultItem>(sql, new { query.ApplicationId });
            result.Items = items.ToArray();
            return result;
        }
    }
}