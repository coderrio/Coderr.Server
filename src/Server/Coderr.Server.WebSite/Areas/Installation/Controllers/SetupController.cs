using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Infrastructure;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.WebSite.Areas.Installation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coderr.Server.WebSite.Areas.Installation.Controllers
{
    [Area("Installation")]

    public class SetupController : Controller
    {
        private ConfigurationStore _configStore;

        public SetupController(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Activate()
        {
            if (!SetupTools.DbTools.IsConfigurationComplete(HostConfig.Instance.ConnectionString))
            {
                return RedirectToAction("Completed", new
                {
                    displayError = 1
                });
            }

            HostConfig.Instance.TriggeredConfigured();
            SetupTools.DbTools.MarkConfigurationAsComplete();
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
            HostConfig.Instance.TriggeredConfigured();
            SetupTools.DbTools.MarkConfigurationAsComplete();
            return View();
        }

        public ActionResult Stats()
        {
            return View();
        }

        public ActionResult Errors()
        {
            var model = new ErrorTrackingViewModel();
            var config = _configStore.Load<CoderrConfigSection>();
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

            var settings = new CoderrConfigSection
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
            ViewBag.Ready = HostConfig.Instance.IsConfigured;
            return View();
        }



        [HttpPost]
        public ActionResult Index(string key)
        {
            if (key == "changeThis")
            {
                var errMsg = "The configuration password can be found in appSettings.json.";
                ModelState.AddModelError("", errMsg);
                return View();
            }

            if (key != HostConfig.Instance.InstallationPassword)
            {
                var errMsg = "The configuration password can be found in appSettings.json.";
                ModelState.AddModelError("", errMsg);
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
                await client.PostAsync("https://coderr.io/support/register/", content);
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