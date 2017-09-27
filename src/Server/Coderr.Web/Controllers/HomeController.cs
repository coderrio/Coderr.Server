using System;
using System.Web.Mvc;

namespace codeRR.Server.Web.Controllers
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
            if (Request.Url.AbsolutePath.EndsWith("/setup/activate", StringComparison.OrdinalIgnoreCase))
                return Redirect("~/?#/welcome");
            return View();
        }

        [AllowAnonymous]
        public ActionResult ToInstall()
        {
            return RedirectToRoute(new {Controller = "Setup", Area = "Installation"});
        }

 

    }
}