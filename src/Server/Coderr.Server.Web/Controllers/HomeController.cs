using Coderr.Server.Web.Areas.Installation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Coderr.Server.Web.Controllers
{
    [Authorize()]
    public class HomeController : Controller
    {
        private readonly InstallationOptions _installationOptions;

        public HomeController(IOptions<InstallationOptions> installationOptions)
        {
            _installationOptions = installationOptions.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// We need to make sure that the installation wizard can redirect to the install start page.
        /// </remarks>
        [HttpGet]
        public IActionResult Index()
        {
            //if (!_installationOptions.IsConfigured)
            //    return Redirect("~/installation/");

            //if (!User.Identity.IsAuthenticated)
            //    return RedirectToAction("Login", "Account");

            return View();
        }
    }
}