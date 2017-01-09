using System.Web.Mvc;
using OneTrueError.Api.Core.Applications;
using OneTrueError.Api.Core.Applications.Commands;

namespace OneTrueError.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous, Route("installation/{*url}")]
        public ActionResult NoInstallation()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ToInstall()
        {
            return RedirectToRoute(new {Controller = "Setup", Area = "Installation"});
        }
        
    }
}