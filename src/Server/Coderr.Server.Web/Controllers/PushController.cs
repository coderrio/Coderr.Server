using Coderr.Server.Abstractions.Config;
using Coderr.Server.ReportAnalyzer.UserNotifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebPush;

namespace Coderr.Server.Web.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private readonly ConfigurationStore _configurationStore;
        private readonly IConfiguration<BrowserNotificationConfig> _config;

        public PushController(IConfiguration<BrowserNotificationConfig> config, ConfigurationStore configurationStore)
        {
            _configurationStore = configurationStore;
            _config = config;
        }

      
        [HttpGet]
        [Route("vapidpublickey")]
        [Authorize]
        public ActionResult<string> VapidPublicKey()
        {
            if (string.IsNullOrEmpty(_config.Value.PrivateKey))
            {
                var pair = VapidHelper.GenerateVapidKeys();
                _config.Value.PrivateKey = pair.PrivateKey;
                _config.Value.PublicKey = pair.PublicKey;
                _configurationStore.Store(_config.Value);

            }
            return Ok(_config.Value.PublicKey);
        }
    }
}