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

        [HttpGet]
        [Route("vapidpublickey")]
        [Authorize]
        public ActionResult<string> VapidPublicKey()
        {
            var keys = VapidHelper.GenerateVapidKeys();
            return Ok(keys.PublicKey);
        }
    }
}