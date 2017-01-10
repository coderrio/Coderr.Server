using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using OneTrueError.Api.Core.Accounts.Queries;
using Xunit;

namespace OneTrueError.Api.Client.Tests
{
#if DEBUG
    public class TryTheClient
    {
        //[Fact]
        public async Task Test()
        {
            var client = new OneTrueApiClient();
            client.Open(new Uri("http://localhost/onetrueerror/"), "", "");
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