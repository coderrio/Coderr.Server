using Coderr.Server.Web.Areas.Installation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Coderr.Server.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly InstallationOptions _installationOptions;

        public HomeController(IOptions<InstallationOptions> installationOptions)
        {
            _installationOptions = installationOptions.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!_installationOptions.IsConfigured)
                return Redirect("~/installation/");

            return View();
        }
    }
}