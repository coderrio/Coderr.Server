using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications
{
    internal class SetApplicationGroupHandler : IMessageHandler<SetApplicationGroup>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public SetApplicationGroupHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, SetApplicationGroup message)
        {
            await _unitOfWork.DeleteAsync<ApplicationGroupMap>(new { message.ApplicationId });

            var groupId = message.ApplicationGroupId;
            if (!string.IsNullOrWhiteSpace(message.GroupName))
            {
                var group = await _unitOfWork.FirstOrDefaultAsync<ApplicationGroup>(new { Name = message.GroupName });
                if (group == null)
                    throw new InvalidOperationException("There is no group called " + message.GroupName);

                groupId = group.Id;
            }

            var map = new ApplicationGroupMap
            {
                ApplicationGroupId = groupId,
                ApplicationId = message.ApplicationId
            };
            await _unitOfWork.InsertAsync(map);
        }
    }
}