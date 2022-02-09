using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    public class DeleteApplicationGroupHandler : IMessageHandler<DeleteApplicationGroup>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteApplicationGroupHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, DeleteApplicationGroup message)
        {
            var moveToId = message.MoveAppsToGroupId;
            if (message.MoveAppsToGroupId == 0)
            {
                moveToId = (int)_unitOfWork.ExecuteScalar("SELECT TOP(1) Id FROM ApplicationGroups ORDER BY Id");
            }

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = "UPDATE ApplicationGroupMap SET ApplicationGroupId = @toGroupId WHERE ApplicationGroupId = @fromGroupId";
                cmd.AddParameter("toGroupId", moveToId);
                cmd.AddParameter("fromGroupId", message.GroupId);
                cmd.ExecuteNonQuery();
            }

            await _unitOfWork.DeleteAsync<ApplicationGroup>(new { Id = message.GroupId });
        }
    }
}