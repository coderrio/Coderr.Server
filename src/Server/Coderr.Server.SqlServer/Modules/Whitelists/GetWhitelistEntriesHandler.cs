using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelist
{
    public class GetWhitelistEntriesHandler : IQueryHandler<GetWhitelistEntries, GetWhitelistEntriesResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private readonly IEntityMapper<GetWhitelistEntriesResultItem> _mapper =
            new MirrorMapper<GetWhitelistEntriesResultItem>();

        public GetWhitelistEntriesHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetWhitelistEntriesResult> HandleAsync(IMessageContext context, GetWhitelistEntries message)
        {
            List<GetWhitelistEntriesResultItem> entries;
            if (message.ApplicationId != null && !string.IsNullOrWhiteSpace(message.DomainName))
                entries = await _unitOfWork.ToListAsync<GetWhitelistEntriesResultItem>(_mapper, 
                    "ApplicationId = @applicationId AND DomainName = @domainNAme",
                    new {message.ApplicationId, message.DomainName});
            else if (message.ApplicationId != null)
                entries =
                    await _unitOfWork.ToListAsync<GetWhitelistEntriesResultItem>(
                        "ApplicationId = @applicationId",
                        new {message.ApplicationId});
            else
                entries =
                    await _unitOfWork.ToListAsync<GetWhitelistEntriesResultItem>(
                        "DomanName = @domainName",
                        new {message.DomainName});

            return new GetWhitelistEntriesResult {Entries = entries.ToArray()};
        }
    }
}