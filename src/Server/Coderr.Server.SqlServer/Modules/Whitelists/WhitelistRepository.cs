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
            return await _unitOfWork.FirstOrDefaultAsync<WhitelistedDomainIp>(
                "ApplicationId = @applicationId AND IpAddress=@address",
                new {applicationId, address = address.ToString()});
        }

        public async Task<IReadOnlyList<App.Modules.Whitelists.Whitelist>> GetWhitelist(int applicationId)
        {
            var domains = await _unitOfWork.ToListAsync<App.Modules.Whitelists.Whitelist>("ApplicationId = @applicationId",
                new {applicationId});

            var ids = string.Join(", ", domains.Select(x => x.Id));
            var ips = await _unitOfWork.ToListAsync<WhitelistedDomainIp>($"DomainId IN ({ids})");
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