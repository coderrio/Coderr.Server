using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Api.Core.Applications.Events;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    public class CreateApplicationGroupHandler : IMessageHandler<CreateApplicationGroup>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public CreateApplicationGroupHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, CreateApplicationGroup message)
        {
            var entry = await _unitOfWork.FirstOrDefaultAsync<ApplicationGroup>(new {message.Name});
            if (entry != null)
                return;

            var group = new ApplicationGroup
            {
                Name = message.Name
            };
            await _unitOfWork.InsertAsync(group);

            await context.ReplyAsync(
                new ApplicationGroupCreated(group.Id, group.Name, context.Principal.GetAccountId()));
        }
    }
}