using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace codeRR.Server.Web.Areas.Installation.Controllers
{
    /// <summary>
    /// Purpose is to be able to launch installation area and be able to use dependencies in the home controller
    /// </summary>
    public class BootController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous, Route("installation/{*url}")]
        public ActionResult NoInstallation()
        {
            if (Request.Url.AbsolutePath.EndsWith("/setup/activate", StringComparison.OrdinalIgnoreCase))
                return Redirect("~/?#/welcome/admin/");
            return View();
        }

        [AllowAnonymous]
        public ActionResult ToInstall()
        {
            return RedirectToRoute(new { Controller = "Setup", Area = "Installation" });
        }


    }
}