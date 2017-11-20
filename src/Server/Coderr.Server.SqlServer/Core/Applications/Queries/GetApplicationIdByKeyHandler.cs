using System.Threading.Tasks;
using codeRR.Server.Api.Core.Applications.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;

namespace codeRR.Server.SqlServer.Core.Applications.Queries
{
    [Component]
    public class GetApplicationIdByKeyHandler : IQueryHandler<GetApplicationIdByKey, GetApplicationIdByKeyResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public GetApplicationIdByKeyHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GetApplicationIdByKeyResult> HandleAsync(IMessageContext context, GetApplicationIdByKey query)
        {
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT Id FROM Applications WHERE AppKey = @appKey";
                cmd.AddParameter("appKey", query.ApplicationKey);
                var result = await cmd.ExecuteScalarAsync();
                if (result == null)
                    return null;

                return new GetApplicationIdByKeyResult {Id = (int)result };
            }
        }
    }
}