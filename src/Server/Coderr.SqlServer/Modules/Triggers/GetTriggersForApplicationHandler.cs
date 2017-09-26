using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Modules.Triggers;
using OneTrueError.Api.Modules.Triggers.Queries;

namespace OneTrueError.SqlServer.Modules.Triggers
{
    [Component]
    public class GetTriggersForApplicationHandler : IQueryHandler<GetTriggersForApplication, TriggerDTO[]>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetTriggersForApplicationHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TriggerDTO[]> ExecuteAsync(GetTriggersForApplication query)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Triggers WHERE [ApplicationId]=@appId";
                cmd.AddParameter("appId", query.ApplicationId);
                return (await cmd.ToListAsync<TriggerDTO>()).ToArray();
            }
        }
    }
}