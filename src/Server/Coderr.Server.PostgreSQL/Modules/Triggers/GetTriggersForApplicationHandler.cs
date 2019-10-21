using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Triggers;
using Coderr.Server.Api.Modules.Triggers.Queries;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.Triggers
{
    public class GetTriggersForApplicationHandler : IQueryHandler<GetTriggersForApplication, TriggerDTO[]>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetTriggersForApplicationHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TriggerDTO[]> HandleAsync(IMessageContext context, GetTriggersForApplication query)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Triggers WHERE [ApplicationId]=@appId;";
                cmd.AddParameter("appId", query.ApplicationId);
                return (await cmd.ToListAsync<TriggerDTO>()).ToArray();
            }
        }
    }
}