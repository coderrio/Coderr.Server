using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Whitelists.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
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
            var items = new List<GetWhitelistEntriesResultItem>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                CreateSqlStatement(message, cmd);

                var appMap = new Dictionary<int, List<GetWhitelistEntriesResultItemApp>>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        MapRow(reader, items, appMap);
                    }

                    foreach (var map in appMap)
                    {
                        items.First(x => x.Id == map.Key).Applications = map.Value.ToArray();
                    }
                }
            }

            if (!items.Any())
                return new GetWhitelistEntriesResult { Entries = new GetWhitelistEntriesResultItem[0] };

            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                var entryIds = items.Select(x => x.Id).ToArray();
                cmd.CommandText =
                    $"SELECT * FROM  WhitelistedDomainIps WHERE DomainId IN ({string.Join(", ", entryIds)})";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var map = new Dictionary<int, List<GetWhitelistEntriesResultItemIp>>();
                    while (await reader.ReadAsync())
                    {
                        var domainId = (int)reader["DomainId"];
                        if (!map.TryGetValue(domainId, out var ipItems))
                        {
                            ipItems = new List<GetWhitelistEntriesResultItemIp>();
                            map[domainId] = ipItems;
                        }
                        ipItems.Add(new GetWhitelistEntriesResultItemIp
                        {
                            Address = (string)reader["IpAddress"],
                            Type = (ResultItemIpType)(int)reader["IpType"],
                            UpdatedAtUtc = (DateTime)reader["StoredAtUtc"]
                        });
                    }

                    foreach (var kvp in map)
                    {
                        items.First(x => x.Id == kvp.Key).IpAddresses = kvp.Value.ToArray();
                    }
                }
            }

            return new GetWhitelistEntriesResult { Entries = items.ToArray() };
        }

        private static void CreateSqlStatement(GetWhitelistEntries message, DbCommand cmd)
        {
            cmd.CommandText =
                @"SELECT WhitelistedDomains.*, WhitelistedDomainApps.ApplicationId, Applications.Name ApplicationName
                            FROM WhitelistedDomains
                            LEFT JOIN WhitelistedDomainApps ON (DomainId = WhitelistedDomains.Id)
                            LEFT JOIN Applications ON (ApplicationId = Applications.Id)";
            if (message.ApplicationId != null && !string.IsNullOrWhiteSpace(message.DomainName))
            {
                cmd.CommandText += @"
                            WHERE ApplicationId = @applicationId 
                            AND DomainName = @domainName";
                cmd.AddParameter("domainName", message.DomainName);
                cmd.AddParameter("applicationId", message.ApplicationId.Value);
            }
            else if (message.ApplicationId != null)
            {
                cmd.CommandText += @"
                            WHERE ApplicationId = @applicationId";
                cmd.AddParameter("domainName", message.DomainName);
                cmd.AddParameter("applicationId", message.ApplicationId.Value);
            }
            else if (!string.IsNullOrEmpty(message.DomainName))
            {
                cmd.CommandText += @"
                            WHERE DomainName = @domainName";
                cmd.AddParameter("domainName", message.DomainName);
            }
        }

        private static void MapRow(IDataRecord record, ICollection<GetWhitelistEntriesResultItem> items,
            IDictionary<int, List<GetWhitelistEntriesResultItemApp>> appMap)
        {
            var item = items.FirstOrDefault(x => x.Id == record.GetInt32(0));
            if (item == null)
            {
                item = new GetWhitelistEntriesResultItem
                {
                    Id = record.GetInt32(0),
                    DomainName = (string)record["DomainName"]
                };
                items.Add(item);
                appMap[item.Id] = new List<GetWhitelistEntriesResultItemApp>();
            }

            var appIdValue = record["ApplicationId"];
            if (appIdValue is DBNull)
                return;

            var app = new GetWhitelistEntriesResultItemApp
            {
                ApplicationId = (int)appIdValue,
                Name = (string)record["ApplicationName"]
            };
            appMap[item.Id].Add(app);
        }
    }
}