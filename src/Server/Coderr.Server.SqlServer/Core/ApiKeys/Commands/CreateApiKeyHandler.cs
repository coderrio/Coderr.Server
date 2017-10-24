using System;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.ApiKeys.Commands;
using codeRR.Server.Api.Core.ApiKeys.Events;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;

namespace codeRR.Server.SqlServer.Core.ApiKeys.Commands
{
    [Component(RegisterAsSelf = true)]
    public class CreateApiKeyHandler : IMessageHandler<CreateApiKey>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IMessageBus _messageBus;

        public CreateApiKeyHandler(IAdoNetUnitOfWork unitOfWork, IMessageBus messageBus)
        {
            _unitOfWork = unitOfWork;
            _messageBus = messageBus;
        }

        public async Task HandleAsync(IMessageContext context, CreateApiKey command)
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
            await context.SendAsync(evt);
        }
    }
}