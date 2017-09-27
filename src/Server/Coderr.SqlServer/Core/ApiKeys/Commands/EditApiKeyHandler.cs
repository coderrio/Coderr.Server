using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using codeRR.Api.Core.ApiKeys.Commands;
using codeRR.Api.Core.ApiKeys.Events;

namespace codeRR.SqlServer.Core.ApiKeys.Commands
{
    [Component(RegisterAsSelf = true)]
    public class EditApiKeyHandler : ICommandHandler<EditApiKey>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IEventBus _eventBus;

        public EditApiKeyHandler(IAdoNetUnitOfWork unitOfWork, IEventBus eventBus)
        {
            _unitOfWork = unitOfWork;
            _eventBus = eventBus;
        }

        public async Task ExecuteAsync(EditApiKey command)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    "UPDATE ApiKeys SET ApplicationName=@appName WHERE Id = @id";
                cmd.AddParameter("appName", command.ApplicationName);
                cmd.AddParameter("id", command.Id);
                await cmd.ExecuteNonQueryAsync();
            }


            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "DELETE FROM ApiKeyApplications WHERE ApiKeyId = @id";
                cmd.AddParameter("id", command.Id);
                await cmd.ExecuteNonQueryAsync();
            }

            foreach (var applicationId in command.ApplicationIds)
            {
                using (var cmd = _unitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO ApiKeyApplications (ApiKeyId, ApplicationId) VALUES(@key, @app)";
                    cmd.AddParameter("app", applicationId);
                    cmd.AddParameter("key", command.Id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            //TODO: Update event?
        }
    }
}