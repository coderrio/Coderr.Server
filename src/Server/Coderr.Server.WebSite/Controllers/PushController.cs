using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Coderr.Server.WebPush.Model;
using Coderr.Server.WebPush.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly ConfigurationStore _configurationStore;
        private readonly IConfiguration<BrowserNotificationConfig> _config;
        private readonly IConfiguration<BaseConfiguration> _baseConfig;

        public PushController(IConfiguration<BrowserNotificationConfig> config, ConfigurationStore configurationStore, IConfiguration<BaseConfiguration> baseConfig)
        {
            _configurationStore = configurationStore;
            _baseConfig = baseConfig;
            _config = config;
        }


        [HttpGet]
        [Route("vapidpublickey")]
        [Authorize]
        public ActionResult<string> VapidPublicKey()
        {
            if (!string.IsNullOrEmpty(_config.Value.PrivateKey))
            {
                return Ok(_config.Value.PublicKey);
            }

            var subject = ServerConfig.Instance.IsLive
                ? VapidDetails.LiveSubject
                : $"mailto:{_baseConfig.Value.SupportEmail}";
            var pair = VapidHelper.GenerateVapidKeys(subject);
            _config.Value.PrivateKey = pair.PrivateKey;
            _config.Value.PublicKey = pair.PublicKey;
            _configurationStore.Store(_config.Value);

            return Ok(_config.Value.PublicKey);
        }
    }
}