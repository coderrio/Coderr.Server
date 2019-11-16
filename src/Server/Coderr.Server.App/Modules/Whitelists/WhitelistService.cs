using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.Applications;
using DnsClient;
using DnsClient.Protocol;

namespace Coderr.Server.App.Modules.Whitelists
{
    /// <summary>
    ///     Used to validate origin of inbound requests when a shared secret is not used.
    /// </summary>
    [ContainerService]
    public class WhitelistService : IWhitelistService
    {
        private readonly IWhitelistRepository _repository;
        private readonly IApplicationRepository _applicationRepository;

        public WhitelistService(IWhitelistRepository repository, IApplicationRepository applicationRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _applicationRepository = applicationRepository;
        }

        public async Task<bool> Validate(string appKey, IPAddress remoteAddress)
        {
            var app = await _applicationRepository.GetByKeyAsync(appKey);
            return await Validate(app.Id, remoteAddress);
        }

        /// <summary>
        ///     Is domain white listed?
        /// </summary>
        /// <param name="applicationId">Application that the error is reported for.</param>
        /// <param name="remoteAddress">IP address of the client reporting the error.</param>
        /// <returns></returns>
        public async Task<bool> Validate(int applicationId, IPAddress remoteAddress)
        {
            var ipEntry = await _repository.FindIp(applicationId, remoteAddress);
            if (ipEntry != null) return ipEntry.IpType != IpType.Denied;

            var domains = await _repository.FindWhitelists(applicationId);

            // Allow nothing if the whitelist is empty
            if (!domains.Any())
                return false;

            foreach (var domain in domains)
            {
                var found = await Lookup(domain, remoteAddress);
                if (found)
                    return true;
            }

            foreach (var domain in domains)
            {
                await _repository.SaveIp(new WhitelistedDomainIp
                {
                    IpAddress = remoteAddress,
                    DomainId = domain.Id,
                    IpType = IpType.Denied,
                    StoredAtUtc = DateTime.UtcNow
                });
            }

            return false;
        }

        private async Task<bool> Lookup(Whitelist domain, IPAddress remoteAddress)
        {
            var found = false;
            var lookup = new LookupClient();
            var result = await lookup.QueryAsync(domain.DomainName, QueryType.ANY);
            foreach (var record in result.AllRecords)
            {
                switch (record)
                {
                    case ARecord ipRecord:
                        if (domain.IpAddresses.Any(x => Equals(x.IpAddress, ipRecord.Address)))
                            continue;


                        if (remoteAddress.Equals(ipRecord.Address))
                            found = true;

                        await _repository.SaveIp(new WhitelistedDomainIp
                        {
                            DomainId = domain.Id,
                            IpAddress = ipRecord.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;

                    case AaaaRecord ip6Record:
                        if (domain.IpAddresses.Any(x => Equals(x.IpAddress, ip6Record.Address)))
                            continue;

                        if (remoteAddress.Equals(ip6Record.Address))
                            found = true;

                        await _repository.SaveIp(new WhitelistedDomainIp
                        {
                            DomainId = domain.Id,
                            IpAddress = ip6Record.Address,
                            IpType = IpType.Lookup,
                            StoredAtUtc = DateTime.UtcNow
                        });
                        break;
                }
            }

            return found;
        }
    }
}