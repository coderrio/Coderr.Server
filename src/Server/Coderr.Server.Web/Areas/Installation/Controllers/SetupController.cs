using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.Web.Areas.Installation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Coderr.Server.Web.Areas.Installation.Controllers
{
    [Area("Installation")]

    public class SetupController : Controller
    {
        private readonly IOptions<InstallationOptions> _config;
        private ConfigurationStore _configStore;
        private readonly IConfiguration _configuration;

        public SetupController(IOptions<InstallationOptions> config, ConfigurationStore configStore, IConfiguration configuration)
        {
            _config = config;
            _configStore = configStore;
            this._configuration = configuration;
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Activate()
        {
            if (!_config.Value.IsConfigured)
                return RedirectToAction("Completed", new
                {
                    displayError = 1
                });

            return Redirect("~/");
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
                model.BaseUrl = Request.GetDisplayUrl()
                    .Replace("installation/setup/basics/", "")
                    .Replace("localhost", "yourServerName");
                ViewBag.NextLink = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Basics(BasicsViewModel model)
        {
            if (model.BaseUrl.StartsWith("http://yourServerName", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct server name in the URL, or all links in notification emails will be incorrect.");
            if (model.BaseUrl.StartsWith("http://localhost", StringComparison.OrdinalIgnoreCase))
                ModelState.AddModelError("BaseUrl", "You must specify a correct server name in the URL, or all links in notification emails will be incorrect.");
            if (model.BaseUrl.StartsWith("https://localhost", StringComparison.OrdinalIgnoreCase))
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
            InstallAuthorizationFilter.IsInstallationCompleted = true;
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
                var con = new SqlConnection(_configuration.GetConnectionString("Db"));
                con.Open();
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
            if (key == "changeThis")
            {
                ModelState.AddModelError("",
                    "Change the 'Installation/Password' setting in appsettings.json and then try again.");
                return View();
            }

            if (key != _config.Value.Password)
            {
                ModelState.AddModelError("",
                    "Enter the 'Installation/Password' value from appsettings.json and then try again.");
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

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Response.Headers.Add("Cache-Control", "no-cache, no-store");
            Response.Headers.Add("Expires", "-1");
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            return base.OnActionExecutionAsync(context, next);
        }
    }
}