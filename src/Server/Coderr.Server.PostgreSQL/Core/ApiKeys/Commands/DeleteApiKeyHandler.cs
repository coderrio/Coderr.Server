using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.ApiKeys.Commands;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;

namespace Coderr.Server.PostgreSQL.Core.ApiKeys.Commands
{
    public class DeleteApiKeyHandler : IMessageHandler<DeleteApiKey>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteApiKeyHandler(IAdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));
            _unitOfWork = unitOfWork;
        }

        public Task HandleAsync(IMessageContext context, DeleteApiKey command)
        {
            int id;
            if (!string.IsNullOrEmpty(command.ApiKey))
            {
                id =
                    (int)
                        _unitOfWork.ExecuteScalar("SELECT Id FROM ApiKeys WHERE GeneratedKey = @key;",
                            new {key = command.ApiKey});
            }
            else
            {
                id = command.Id;
            }

            _unitOfWork.ExecuteNonQuery("DELETE FROM [ApiKeyApplications] WHERE ApiKeyId = @id;", new {id});
            _unitOfWork.ExecuteNonQuery("DELETE FROM [ApiKeys] WHERE Id = @id;", new {id});
            return Task.FromResult<object>(null);
        }
    }
}