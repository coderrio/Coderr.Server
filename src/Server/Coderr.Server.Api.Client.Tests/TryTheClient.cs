using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Accounts.Queries;
using FluentAssertions;

namespace codeRR.Server.Api.Client.Tests
{
#if DEBUG
    public class TryTheClient
    {
        //[Fact]
        public async Task Test()
        {
            var client = new ServerApiClient();
            client.Open(new Uri("http://localhost/coderr/"), "", "");
            FindAccountByUserNameResult result = null;
            try
            {
                result = await client.QueryAsync(new FindAccountByUserName("admin"));
            }
            catch (WebException)
            {
            }


            result.Should().NotBeNull();
        }
    }
#endif
}