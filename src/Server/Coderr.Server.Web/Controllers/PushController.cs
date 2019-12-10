using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class PushController : ControllerBase
    {
        private PushService _pushService;

        public PushController(PushService pushService)
        {
            _pushService = pushService;
        }

        [HttpGet, Route("vapidpublickey")]
        [Authorize]
        public PushService.VapidKey VapidPublicKey()
        {
            return _pushService.Generate();
        }

    }
}
