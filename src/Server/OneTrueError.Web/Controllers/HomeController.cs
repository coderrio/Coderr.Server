using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ToInstall()
        {
            return RedirectToRoute(new {Controller = "Setup", Area = "Installation"});
            //return Redirect("~/Installation");

        }

        [AllowAnonymous, Route("installation/{*url}")]
        public ActionResult NoInstallation()
        {
            return View();
        }

    
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}