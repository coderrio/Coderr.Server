using System;
using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    [System.Web.Http.Authorize]
    public class GuidController : Controller
    {
        [Route("guid/"), HttpGet]
        public ActionResult New()
        {
            return new ContentResult(){Content = Guid.NewGuid().ToString("N"), ContentType = "text/plain"};
        }
    }
}