using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coderr.IntegrationTests.Core.Tools;
using Coderr.Server.Api.Client;
using Coderr.Tests.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.IntegrationTests.Core
{
    public class CoderrFixture : IAssemblyFixture
    {
        private ApplicationClient _client;
        private ServerApiClient _apiClient;
        public const string DbName = "Coderr99";
        public const string ServerAddress = "http://localhost:50473";

        public CoderrFixture()
        {
            _client = new ApplicationClient(ServerAddress, DbName);
        }

        public async Task Prepare()
        {
            _apiClient = await _client.Open();
        }


        public void RegisterServices(IServiceCollection builder)
        {
            builder.AddSingleton(_client);
            builder.AddSingleton<ServerApiClient>(x => _apiClient);
        }

        public Task Cleanup()
        {
            return Task.CompletedTask;
        }
    }
}
