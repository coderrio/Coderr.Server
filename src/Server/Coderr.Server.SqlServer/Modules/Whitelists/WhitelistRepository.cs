using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Modules.Whitelists;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Whitelists
{
    [ContainerService]
    public class WhitelistRepository : IWhitelistRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public WhitelistRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<WhitelistedDomainIp> FindIp(int applicationId, IPAddress address)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                // ORDER BY appId desc = get specific one first, then the generic one.
                cmd.CommandText = @"select ip.* 
                                    from WhitelistedDomains d
                                    left join WhitelistedDomainIps ip ON (d.Id = ip.DomainId)
                                    left join WhitelistedDomainApps app ON (d.Id = app.DomainId)
                                    WHERE ip.IpAddress = @ip
                                    AND (app.ApplicationId = @appId OR app.ApplicationId is NULL)
                                    order by app.ApplicationId desc";
                cmd.AddParameter("ip", address.ToString());
                cmd.AddParameter("appId", applicationId);

                return await cmd.FirstOrDefaultAsync<WhitelistedDomainIp>();
            }
        }

        public async Task<IReadOnlyList<App.Modules.Whitelists.Whitelist>> FindWhitelists(int applicationId)
        {
            var domains = new List<Whitelist>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                // ORDER BY appId desc = get specific one first, then the generic one.
                cmd.CommandText = @"select d.* 
                                    from WhitelistedDomains d
                                    left join WhitelistedDomainApps app ON (d.Id = app.DomainId)
                                    WHERE app.ApplicationId = @appId OR app.ApplicationId is NULL
                                    order by app.ApplicationId desc";
                cmd.AddParameter("appId", applicationId);

                var entries= await cmd.ToListAsync<Whitelist>();
                foreach (var entry in entries)
                {
                    if (domains.Any(x => x.Id == entry.Id))
                        continue;
                    domains.Add(entry);
                }
            }

            if (!domains.Any())
                return domains;

            var ids = string.Join(", ", domains.Select(x => x.Id));
            var ips = await _unitOfWork.ToListAsync<WhitelistedDomainIp>($"SELECT * FROM WhitelistedDomainIps WHERE DomainId IN ({ids})");
            var ipsPerDomain = ips.GroupBy(x => x.DomainId);
            foreach (var domainIps in ipsPerDomain)
            {
                var domain = domains.First(x => x.Id == domainIps.Key);
                domain.IpAddresses = domainIps.ToArray();
            }

            return domains;
        }

        public async Task SaveIp(WhitelistedDomainIp entry)
        {
            await _unitOfWork.InsertAsync(entry);
        }
    }
}