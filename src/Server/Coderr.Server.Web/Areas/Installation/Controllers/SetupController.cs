using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure;
using codeRR.Server.Web.Areas.Installation.Models;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.Web.Areas.Installation.Controllers
{
    [OutputCache(Duration = 0, NoStore = true)]
    public class SetupController : Controller
    {
        private ConfigurationStore _configStore;

        public SetupController()
        {
            _configStore = Startup.ConfigurationStore;
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Activate()
        {
            ConfigurationManager.RefreshSection("appSettings");
            if (ConfigurationManager.AppSettings["Configured"] != "true")
                return RedirectToAction("Completed", new
                {
                    displayError = 1
                });
            return Redirect("~/?#/welcome/admin/");
        }

        public ActionResult Basics()
        {
            var model = new BasicsViewModel();
            var config = _configStore.Load<BaseConfiguration>();
            if (config.BaseUrl != null)
            {
                model.BaseUrl = config.BaseUrl.ToString();
                model.SupportEmail = config.SupportEmail;
            }
            else
            {
                model.BaseUrl = Request.Url.ToString().Replace("installation/setup/basics/", "")
                    .Replace("localhost", "yourServerName");
                ViewBag.NextLink = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Basics(BasicsViewModel model)
        {
            if (model.BaseUrl.StartsWith("http://yourServerName/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct server name in the URL, or all links in notification emails will be incorrect.");
            if (model.BaseUrl.StartsWith("http://localhost/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct server name in the URL, or all links in notification emails will be incorrect.");
            if (model.BaseUrl.StartsWith("https://localhost/", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct server name in the URL, or all links in notification emails will be incorrect.");

            if (!ModelState.IsValid)
            {
                ViewBag.NextLink = "";
                return View(model);
            }

            var settings = new BaseConfiguration();
            if (!model.BaseUrl.EndsWith("/"))
                model.BaseUrl += "/";

            if (model.BaseUrl.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) != -1)
            {
                ModelState.AddModelError("BaseUrl",
                    "Use the servers real DNS name instead of 'localhost'. If you don't the Ajax request wont work as CORS would be enforced by IIS.");
                return View(model);
            }
            settings.BaseUrl = new Uri(model.BaseUrl);
            settings.SupportEmail = model.SupportEmail;
            _configStore.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }


        public ActionResult Completed(string displayError = null)
        {
            ViewBag.DisplayError = displayError == "1";
            return View();
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
            {
                ViewBag.NextLink = "";
            }

            return View("ErrorTracking", model);
        }

        [HttpPost]
        public ActionResult Errors(ErrorTrackingViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ErrorTracking", model);

            var settings = new codeRRConfigSection
            {
                ActivateTracking = true,
                ContactEmail = model.ContactEmail,
                InstallationId = Guid.NewGuid().ToString("N")
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

        [HttpPost]
        public ActionResult Index(string key)
        {
            if (key == "change_this_to_your_own_password_before_running_the_installer")
            {
                ModelState.AddModelError("",
                    "Change the 'ConfigurationKey' appSetting in web.config and then try again.");
                return View();
            }

            if (key != ConfigurationManager.AppSettings["ConfigurationKey"])
            {
                ModelState.AddModelError("",
                    "Enter the value from the 'ConfigurationKey' appSetting in web.config and then try again.");
                return View();
            }

            return Redirect(Url.GetNextWizardStep());
        }

        public ActionResult Support()
        {
            return View(new SupportViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> Support(SupportViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = new HttpClient();
                var content =
                    new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("EmailAddress", model.Email),
                        new KeyValuePair<string, string>("CompanyName", model.CompanyName)
                    });
                await client.PostAsync("https://coderrapp.com/support/register/", content);
                return Redirect(Url.GetNextWizardStep());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }
    }
}