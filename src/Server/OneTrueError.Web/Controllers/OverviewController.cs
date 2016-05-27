using System.Web.Mvc;
using DotNetCqs;

namespace OneTrueError.Web.Controllers
{
    public class OverviewController : Controller
    {
        private IQueryBus _queryBus;

        public OverviewController(IQueryBus queryBus)
        {
            _queryBus = queryBus;
        }

        [Route("overview")]
        public ActionResult Overview()
        {
            return View();
        }
    }
}