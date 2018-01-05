using System;
using System.Web.Mvc;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure;
using codeRR.Server.Web.Areas.Admin.Models;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ConfigurationStore _configStore;

        public HomeController(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        public ActionResult Basics()
        {
            var model = new BasicsViewModel();
            var config = _configStore.Load<BaseConfiguration>();
            if (config.BaseUrl != null)
            {
                model.BaseUrl = config.BaseUrl.ToString();
                model.SupportEmail = config.SupportEmail;
                model.AllowRegistrations = config.AllowRegistrations != false;
            }
            else
            {
                model.AllowRegistrations = true;
                model.BaseUrl = Request.Url.ToString().Replace("installation/setup/basics/", "");
                ViewBag.NextLink = "";
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Basics(BasicsViewModel model)
        {
            if (model.BaseUrl.StartsWith("http://yourServerName/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct URL, or all links in notification emails will be incorrect.");

            if (!ModelState.IsValid)
            {
                ViewBag.NextLink = "";
                return View(model);
            }

            var settings = new BaseConfiguration
            {
                BaseUrl = new Uri(model.BaseUrl),
                SupportEmail = model.SupportEmail,
                AllowRegistrations = model.AllowRegistrations
            };
            _configStore.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }

        public ActionResult Errors()
        {
            var model = new ErrorTrackingViewModel();
            var config = _configStore.Load<codeRRConfigSection>();
            if (!string.IsNullOrEmpty(model.ContactEmail))
            {
                model.ContactEmail = config.ContactEmail;
            }
            else
                ViewBag.NextLink = "";

            return View("ErrorTracking", model);
        }

        [HttpPost]
        public ActionResult Errors(ErrorTrackingViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ErrorTracking", model);

            var settings = new codeRRConfigSection
            {
                ContactEmail = model.ContactEmail,
            };
            _configStore.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }

        // GET: Installation/Home
        public ActionResult Index()
        {
            try
            {
                DbConnectionFactory.Open(true);
            }
            catch
            {
                ViewBag.Ready = false;
            }
            return View();
        }
    }
}