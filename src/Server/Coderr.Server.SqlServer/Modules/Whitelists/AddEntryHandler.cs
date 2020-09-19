using System;
using System.Net;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Commands;
using Coderr.Server.App.Modules.Whitelists;
using DnsClient;
using DnsClient.Protocol;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    internal class AddEntryHandler : IMessageHandler<AddEntry>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(AddEntryHandler));
        private readonly IAdoNetUnitOfWork _uow;

        public AddEntryHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, AddEntry message)
        {
            var entry = new Whitelist {DomainName = message.DomainName};
            await _uow.InsertAsync(entry);

            foreach (var dto in message.ApplicationIds)
            {
                var entity = new WhitelistedDomainApplication {DomainId = entry.Id, ApplicationId = dto};
                await _uow.InsertAsync(entity);
            }

            if (message.IpAddresses?.Length > 0)
            {
                foreach (var ip in message.IpAddresses)
                {
                    var entity = new WhitelistedDomainIp
                    {
                        DomainId = entry.Id,
                        IpType = IpType.Manual,
                        IpAddress = IPAddress.Parse(ip),
                        StoredAtUtc = DateTime.UtcNow
                    };

                    await _uow.InsertAsync(entity);
                }
            }
            else
                await LookupIps(message, entry);
        }

        private async Task LookupIps(AddEntry message, Whitelist entry)
        {
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