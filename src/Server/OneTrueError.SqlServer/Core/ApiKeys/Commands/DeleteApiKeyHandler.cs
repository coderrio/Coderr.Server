using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.ApiKeys.Commands;
using OneTrueError.App.Core.ApiKeys;

namespace OneTrueError.SqlServer.Core.ApiKeys.Commands
{
    [Component(RegisterAsSelf = true)]
    public class DeleteApiKeyHandler : ICommandHandler<DeleteApiKey>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public DeleteApiKeyHandler(IAdoNetUnitOfWork unitOfWork)
        {
            if (unitOfWork == null) throw new ArgumentNullException(nameof(unitOfWork));
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(DeleteApiKey command)
        {
            if (string.IsNullOrEmpty(command.ApiKey))
                await _unitOfWork.DeleteAsync<ApiKey>(command.Id);
            else
                await _unitOfWork.DeleteAsync<ApiKey>(new {GeneratedKey = command.ApiKey});
        }
    }
}