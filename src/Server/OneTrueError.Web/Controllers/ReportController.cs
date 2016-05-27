using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    public class ReportController : Controller
    {
        [Route("incident/{incidentId}/report/{reportId}")]
        public ActionResult Report(int incidentId, int reportId)
        {
            ViewBag.IncidentId = incidentId;
            ViewBag.ReportId = reportId;
            return View();
        }
    }
}