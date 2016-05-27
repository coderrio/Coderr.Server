using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    public class IncidentController : Controller
    {
        [Route("incidents")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("incident/{id}")]
        public ActionResult Incident(int id)
        {
            ViewBag.IncidentId = id;
            return View();
        }

        [Route("incident/{id}/context")]
        public ActionResult Context(int id)
        {
            ViewBag.IncidentId = id;
            return View();
        }
    }
}