using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    public class RenameGroupHandler : IMessageHandler<RenameApplicationGroup>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public RenameGroupHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public Task HandleAsync(IMessageContext context, RenameApplicationGroup message)
        {
            _unitOfWork.Execute("UPDATE ApplicationGroups SET Name = @name WHERE Id = @id",
                new {name = message.NewName, id = message.GroupId});
            return Task.CompletedTask;
        }
    }
}