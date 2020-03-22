using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Commands;
using Coderr.Server.Api.Modules.Whitelists.Queries;
using Coderr.Server.App.Modules.Whitelists;
using DnsClient;
using DnsClient.Protocol;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;
using log4net;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    internal class EditEntryHandler : IMessageHandler<EditEntry>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(EditEntryHandler));
        private readonly IAdoNetUnitOfWork _uow;

        public EditEntryHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(IMessageContext context, EditEntry message)
        {
            var entry = await _uow.FirstAsync<Whitelist>(new { message.Id });

            await FetchIps(entry);
            await FetchApplications(entry);

            await UpdateApplications(message, entry);
            await UpdateIps(message, entry);
        }

        private async Task FetchIps(Whitelist entry)
        {
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    $"SELECT * FROM  WhitelistedDomainIps WHERE DomainId = @domainId";
                cmd.AddParameter("domainId", entry.Id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var addresses = new List<WhitelistedDomainIp>();
                    while (await reader.ReadAsync())
                    {
                        addresses.Add(new WhitelistedDomainIp()
                        {
                            DomainId = entry.Id,
                            IpAddress = IPAddress.Parse((string) reader["IpAddress"]),
                            IpType = (IpType) (int) reader["IpType"],
                            StoredAtUtc = (DateTime) reader["StoredAtUtc"]
                        });
                    }

                    entry.IpAddresses = addresses.ToArray();
                }
            }
        }

        private async Task FetchApplications(Whitelist entry)
        {
            using (var cmd = _uow.CreateDbCommand())
            {
                cmd.CommandText =
                    $"SELECT * FROM  WhitelistedDomainApps WHERE DomainId = @domainId";
                cmd.AddParameter("domainId", entry.Id);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var apps = new List<int>();
                    while (await reader.ReadAsync())
                    {
                        apps.Add((int) reader["ApplicationId"]);
                    }

                    entry.ApplicationIds = apps.ToArray();
                }
            }
        }

        private async Task LookupIps(EditEntry message, Whitelist whitelist)
        {
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(whitelist.DomainName, QueryType.ANY);
            foreach (var record in result.AllRecords)
            {
                switch (record)
                {
                    case ARecord ipRecord:
                        await _uow.InsertAsync(new WhitelistedDomainIp
                        {
                            DomainId = whitelist.Id,
                            IpAddress = ipRecord.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;
                    case AaaaRecord ip6Record:
                        await _uow.InsertAsync(new WhitelistedDomainIp
                        {
                            DomainId = whitelist.Id,
                            IpAddress = ip6Record.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;
                }
            }
        }

        private async Task UpdateApplications(EditEntry message, Whitelist entry)
        {
            var dbApps = await _uow.ToListAsync<WhitelistedDomainApplication>("DomainId = @id", new { id = message.Id });

            //find new
            var newApps = message.ApplicationIds.Except(dbApps.Select(x => x.ApplicationId));
            foreach (var newApp in newApps)
            {
                var entity = new WhitelistedDomainApplication { DomainId = entry.Id, ApplicationId = newApp };
                await _uow.InsertAsync(entity);
            }

            //find removed
            var removedApps = dbApps.Select(x => x.ApplicationId)
                .Except(message.ApplicationIds)
                .Select(x => dbApps.First(y => x == y.ApplicationId));
            foreach (var app in removedApps) await _uow.DeleteAsync(app);
        }

        private async Task UpdateIps(EditEntry message, Whitelist whitelist)
        {
            var dbIps = await _uow.ToListAsync<WhitelistedDomainIp>("DomainId = @id", new { id = message.Id });

            foreach (var address in message.IpAddresses)
            {
                var dbIp = dbIps.FirstOrDefault(x => x.IpAddress.ToString() == address);
                if (dbIp == null)
                {
                    var entity = new WhitelistedDomainIp
                    {
                        DomainId = whitelist.Id,
                        IpType = IpType.Manual,
                        IpAddress = IPAddress.Parse(address),
                        StoredAtUtc = DateTime.UtcNow
                    };
                    await _uow.InsertAsync(entity);
                    continue;
                }

                if (dbIp.IpType == IpType.Manual)
                    continue;

                dbIp.IpType = IpType.Manual;
                await _uow.UpdateAsync(dbIp);

            }

            //find removed
            var removedEntries = dbIps.Select(x => x.IpAddress.ToString())
                .Except(message.IpAddresses)
                .Select(x => dbIps.First(y => x == y.IpAddress.ToString()));
            foreach (var entry in removedEntries)
                await _uow.DeleteAsync(entry);
        }
    }
}