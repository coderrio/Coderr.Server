using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Controllers
{
    [Route("api/[controller]")]
    public class OnboardingController : Controller
    {
        private IConfiguration<BaseConfiguration> _baseConfiguration;

        public OnboardingController(IConfiguration<BaseConfiguration> baseConfiguration)
        {
            _baseConfiguration = baseConfiguration;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Libraries()
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync("https://coderr.io/configure/packages/");
            return Content(result, "application/json", Encoding.UTF8);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> Library(string id, string appKey)
        {
            var hash = CalculateMd5Hash(appKey);
            var uri =
                $"https://coderr.io/client/{id}/configure/{hash}";
            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            try
            {
                var html = await client.GetStringAsync(uri);

                var url = _baseConfiguration.Value.BaseUrl.ToString()
                    .Replace("https://app.", "https://report.")
                    .Replace("https://lobby.", "https://report.")
                    .Replace("http://live-test-lobby", "http://live-test-receiver");

                return Content(html
                    //.Replace("yourAppKey", appInfo.AppKey)
                    //.Replace("yourSharedSecret", appInfo.SharedSecret)
                    .Replace("http://yourServer/coderr/", url)
                    .Replace("\"/documentation/", "\"https://coderr.io/documentation/"), "text/plain");
            }
            catch (HttpRequestException)
            {
                return Content(
                    @"Cannot reach coderr.io, read the documentation to see how to configure Coderr or contact help@coderr.io.<br>
<br>
AppKey: yourAppKey<br>
Shared secret: yourSharedSecret");
            }

        }

        private string CalculateMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            return sb.ToString();
        }
    }
}
