using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.ReportAnalyzer.Abstractions.Inbound.Whitelists.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.ReportAnalyzer.Inbound.Whitelist
{
    public class GetWhitelistEntriesHandler : IQueryHandler<GetWhitelistEntries, GetWhitelistEntriesResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetWhitelistEntriesHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetWhitelistEntriesResult> HandleAsync(IMessageContext context, GetWhitelistEntries message)
        {
            List<GetWhitelistEntriesResultItem> entries;
            if (message.ApplicationId != null && !string.IsNullOrWhiteSpace(message.DomainName))
                entries = await _unitOfWork.ToListAsync<GetWhitelistEntriesResultItem>(
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