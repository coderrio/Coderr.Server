using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.Entities;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;
using Coderr.Server.Api.Modules.Whitelists.Commands;
using Coderr.Server.Api.Modules.Whitelists.Queries;
using Coderr.Tests;
using Coderr.Tests.Attributes;
using FluentAssertions;

namespace Coderr.IntegrationTests.Core.TestCases
{
    public class WhitelistTests
    {
        private readonly ServerApiClient _apiClient;
        private readonly ApplicationClient _applicationClient;

        public WhitelistTests(ApplicationClient applicationClient, ServerApiClient apiClient)
        {
            _applicationClient = applicationClient;
            _apiClient = apiClient;
        }

        [TestInit]
        public async Task Reset()
        {
            var query = new GetWhitelistEntries();
            var result = await _apiClient.QueryAsync(query);
            foreach (var item in result.Entries)
            {
                var cmd = new RemoveEntry
                {
                    Id = item.Id
                };
                await _apiClient.SendAsync(cmd);
            }
        }

        [Test]
        public async Task Should_be_able_to_add_a_whitelist_entry_with_apps()
        {
            var cmd = new AddEntry
            {
                DomainName = "app.coderr.io",
                ApplicationIds = new[] { 1 }
            };
            await _apiClient.SendAsync(cmd);

            var actual = await GetEntry(x => x.DomainName == cmd.DomainName && x.IpAddresses.Any());

            actual.Applications.Single().ApplicationId.Should().Be(1);
            actual.IpAddresses.First().Address.Should().Be("40.69.200.124");
        }

        [Test]
        public async Task Should_be_able_to_add_a_whitelist_entry_with_ips_and_apps()
        {
            var cmd = new AddEntry
            {
                DomainName = "report3.coderr.io",
                ApplicationIds = new[] { 1 },
                IpAddresses = new[] { "1.2.3.4" }
            };
            await _apiClient.SendAsync(cmd);

            var actual = await GetEntry(x => x.DomainName == cmd.DomainName);

            actual.Applications.Single().ApplicationId.Should().Be(1);
        }

        [Test]
        public async Task Should_be_able_to_add_a_whitelist_entry_with_just_domainName()
        {
            var cmd = new AddEntry
            {
                DomainName = "report.coderr.io"
            };
            await _apiClient.SendAsync(cmd);

            var actual = await GetEntry(x => x.DomainName == cmd.DomainName);

            actual.IpAddresses.First().Address.Should().Be("40.69.200.124");
        }

        [Test]
        public async Task Should_be_able_to_add_a_whitelist_entry_with_manual_ip()
        {
            var cmd = new AddEntry
            {
                DomainName = "report2.coderr.io",
                IpAddresses = new[] { "1.2.3.4" }
            };
            await _apiClient.SendAsync(cmd);

            var actual = await GetEntry(x => x.DomainName == cmd.DomainName);
            actual.IpAddresses.Single().Address.Should().Be("1.2.3.4");
        }



        [Test]
        public async Task Should_be_able_to_delete_an_entry()
        {
            var cmd1 = new AddEntry
            {
                DomainName = "report4.coderr.io",
                ApplicationIds = new[] { 1 },
                IpAddresses = new[] { "1.2.3.4" }
            };
            await _apiClient.SendAsync(cmd1);
            var entry = await GetEntry(x => x.DomainName == cmd1.DomainName);

            var cmd = new RemoveEntry
            { Id = entry.Id };
            await _apiClient.SendAsync(cmd);

            await GetEmptyList();
        }

        [Test]
        public async Task Should_be_able_to_update_an_entry()
        {
            var cmd1 = new AddEntry
            {
                DomainName = "report4.coderr.io",
                ApplicationIds = new[] { 1 },
                IpAddresses = new[] { "1.2.3.4" }
            };
            await _apiClient.SendAsync(cmd1);
            var entry = await GetEntry(x => x.DomainName == cmd1.DomainName);

            var cmd = new EditEntry
            { Id = entry.Id, IpAddresses = new[] { "2.3.4.5" } };
            await _apiClient.SendAsync(cmd);

            await GetEntry(x => x.DomainName == cmd1.DomainName && x.IpAddresses[0].Address == cmd.IpAddresses[0]);
        }

        [Test]
        public async Task Should_not_add_report_when_being_blocked()
        {
            var cmd = new AddEntry
            {
                DomainName = "report4.coderr.io",
                ApplicationIds = new[] { 1 },
                IpAddresses = new[] { "1.2.3.4" }
            };
            await _apiClient.SendAsync(cmd);
            await GetEntry(x => x.DomainName == cmd.DomainName);

            IncidentWrapper entry = null;
            try
            {
                entry = await _applicationClient.CreateIncidentWithoutSignature();
            }
            catch
            {
                // should throw since the report should be ignored.
            }

            entry.Should().BeNull();
        }

        [Test]
        public async Task Should_ban_non_matching_ip()
        {
            var cmd = new AddEntry
            {
                DomainName = "report4.coderr.io",
            };
            await _apiClient.SendAsync(cmd);
            await GetEntry(x => x.DomainName == cmd.DomainName);
            try
            {
                await _applicationClient.CreateIncidentWithoutSignature();
            }
            catch
            {
                // should throw since the report should be ignored.
            }

            await GetEntry(x => x.IpAddresses.Any(y => y.Address == "::1"));
        }

        [Test]
        public async Task Should_add_report_when_ip_is_allowed()
        {
            var cmd = new AddEntry
            {
                DomainName = "report5.coderr.io",
                ApplicationIds = new[] { _applicationClient.ApplicationId },
                IpAddresses = new[] { "::1" }
            };
            await _apiClient.SendAsync(cmd);
            await GetEntry(x => x.DomainName == cmd.DomainName);

            var entry = await _applicationClient.CreateIncidentWithoutSignature();

            entry.Should().NotBeNull();
        }

        private async Task GetEmptyList()
        {
            async Task<bool> DoQuery()
            {
                var query = new GetWhitelistEntries();
                var result = await _apiClient.QueryAsync(query);
                return !(result.Entries?.Length > 1);
            }

            if (!await ActionExtensions.Retry(DoQuery))
                throw new TestFailedException("Whitelist entry was not found.");
        }


        private async Task<GetWhitelistEntriesResultItem> GetEntry(
            Func<GetWhitelistEntriesResultItem, bool> filter = null)
        {
            GetWhitelistEntriesResultItem entry = null;

            async Task<bool> DoQuery()
            {
                var query = new GetWhitelistEntries();
                var result = await _apiClient.QueryAsync(query);
                if (filter == null)
                {
                    entry = result.Entries?.LastOrDefault();
                    return true;
                }

                foreach (var item in result.Entries)
                {
                    if (!filter(item))
                        return false;

                    entry = item;
                    return true;
                }

                return false;
            }

            if (!await ActionExtensions.Retry(DoQuery))
                throw new TestFailedException("Whitelist entry was not found.");

            return entry;
        }
    }
}