using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.SqlServer.Tools;

namespace OneTrueError.SqlServer.Core.Applications.Queries
{
    [Component]
    public class GetApplicationIdByKeyHandler : IQueryHandler<GetApplicationIdByKey, GetApplicationIdByKeyResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public GetApplicationIdByKeyHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GetApplicationIdByKeyResult> ExecuteAsync(GetApplicationIdByKey query)
        {
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText = "SELECT Id FROM Applications WHERE AppKey = @appKey";
                cmd.AddParameter("appKey", query.ApplicationKey);
                return new GetApplicationIdByKeyResult{Id = (int) await cmd.ExecuteScalarAsync()};
            }
        }
    }
}