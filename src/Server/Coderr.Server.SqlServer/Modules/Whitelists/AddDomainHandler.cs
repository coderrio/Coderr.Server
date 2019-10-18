using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Commands;
using Coderr.Server.App.Modules.Whitelists;
using DnsClient;
using DnsClient.Protocol;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.Modules.Whitelist
{
    internal class AddDomainHandler : IMessageHandler<AddDomain>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AddDomainHandler));
        private readonly IAdoNetUnitOfWork _uow;

        public AddDomainHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, AddDomain message)
        {
            var entry = new WhitelistedDomain
            {
                ApplicationId = message.ApplicationId,
                DomainName = message.DomainName
            };
            await _uow.InsertAsync(entry);
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(message.DomainName, QueryType.ANY);
            foreach (var record in result.AllRecords)
            {
                switch (record)
                {
                    case ARecord ipRecord:
                        await _uow.InsertAsync(new WhitelistedDomainIp
                        {
                            DomainId = entry.Id,
                            IpAddress = ipRecord.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;
                    case AaaaRecord ip6Record:
                        await _uow.InsertAsync(new WhitelistedDomainIp
                        {
                            DomainId = entry.Id,
                            IpAddress = ip6Record.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;
                }
            }
        }
    }
}