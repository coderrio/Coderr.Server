using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Queries;
using Coderr.Server.PostgreSQL.Core.ApiKeys.Mappings;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Environments
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
                sql = @"select Id, Name from Environments ORDER BY Name;";
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
                            JOIN EnvironmentIds ON (EnvironmentId=Environments.Id);";
            }

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("applicationId", query.ApplicationId);
                var items = await cmd.ToListAsync<GetEnvironmentsResultItem>();
                result.Items = items.ToArray();
                return result;
            }
        }
    }
}