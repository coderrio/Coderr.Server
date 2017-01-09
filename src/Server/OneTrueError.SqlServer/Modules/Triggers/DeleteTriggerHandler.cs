using System.Data.Common;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.Api.Modules.Triggers.Commands;

namespace OneTrueError.SqlServer.Modules.Triggers
{
    [Component]
    public class DeleteTriggerHandler : ICommandHandler<DeleteTrigger>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public DeleteTriggerHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task ExecuteAsync(DeleteTrigger command)
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