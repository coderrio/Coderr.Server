using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    public class FeedbackController : Controller
    {
        [Route("feedback")]
        public ActionResult Overview()
        {
            return View();
        }

        [Route("application/{appId}/feedback")]
        public ActionResult Application(int appId)
        {
            ViewBag.ApplicationId = appId;
            return View();
        }

        [Route("incident/{incidentId}/feedback")]
        public ActionResult Incident(int incidentId)
        {
            ViewBag.IncidentId = incidentId;
            return View();
        }

    }
}