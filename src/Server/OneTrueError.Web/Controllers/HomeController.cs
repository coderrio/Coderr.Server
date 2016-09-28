using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using OneTrueError.Api.Client;
using OneTrueError.Api.Core.Applications.Queries;

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
        }

        [AllowAnonymous, Route("installation/{*url}")]
        public ActionResult NoInstallation()
        {
            return View();
        }
    }
}