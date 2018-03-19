using System.Data.Common;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Triggers.Commands;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Modules.Triggers
{
    public class DeleteTriggerHandler : IMessageHandler<DeleteTrigger>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public DeleteTriggerHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, DeleteTrigger command)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Triggers WHERE Id = @id";
                cmd.AddParameter("id", command.Id);
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}