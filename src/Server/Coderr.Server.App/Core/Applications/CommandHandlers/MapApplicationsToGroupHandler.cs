using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    class MapApplicationsToGroupHandler : IMessageHandler<MapApplicationsToGroup>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public MapApplicationsToGroupHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task HandleAsync(IMessageContext context, MapApplicationsToGroup message)
        {
            _uow.ExecuteNonQuery($"DELETE FROM ApplicationGroupMap WHERE ApplicationGroupId=@groupId", new { groupId = message.GroupId });

            using (var cmd = _uow.CreateDbCommand())
            {
                var sql = "INSERT INTO ApplicationGroupMap (ApplicationGroupId, ApplicationId) VALUES";
                foreach (var appId in message.ApplicationIds)
                {
                    sql += $"({message.GroupId}, {appId}),\r\n";
                }

                //last ",crlf "
                sql = sql.Remove(sql.Length - 3, 3);
                cmd.CommandText = sql;
                await cmd.ExecuteNonQueryAsync();
            }

            var ids = string.Join(", ", message.ApplicationIds);
            _uow.ExecuteNonQuery($"DELETE FROM ApplicationGroupMap WHERE ApplicationGroupId = 1 AND ApplicationId IN ({ids})");
        }
    }
}
