using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using OneTrueError.Api.Core.ApiKeys.Commands;
using OneTrueError.Api.Core.ApiKeys.Events;

namespace OneTrueError.SqlServer.Core.ApiKeys.Commands
{
    [Component(RegisterAsSelf = true)]
    public class CreateApiKeyHandler : ICommandHandler<CreateApiKey>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public CreateApiKeyHandler(IAdoNetUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task ExecuteAsync(CreateApiKey command)
        {
            int id;
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "INSERT INTO ApiKeys (ApplicationName, GeneratedKey, SharedSecret, CreatedById, CreatedAtUtc) VALUES(@appName, @key, @secret, @by, @when); select cast(scope_identity() as int);";
                cmd.AddParameter("appName", command.ApplicationName);
                cmd.AddParameter("key", command.ApiKey);
                cmd.AddParameter("secret", command.SharedSecret);
                cmd.AddParameter("by", command.AccountId);
                cmd.AddParameter("when", DateTime.UtcNow);
                id = (int) await cmd.ExecuteScalarAsync();
            }


            foreach (var applicationId in command.ApplicationIds)
            {
                using (var cmd = _unitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO ApiKeyApplications (ApiKeyId, ApplicationId) VALUES(@key, @app)";
                    cmd.AddParameter("app", applicationId);
                    cmd.AddParameter("key", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            var evt = new ApiKeyCreated(command.ApplicationName, command.ApiKey, command.SharedSecret,
                command.ApplicationIds, command.AccountId);
            await _eventBus.PublishAsync(evt);
        }
    }
}