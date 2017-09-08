using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Installation.Models;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
    [OutputCache(Duration = 0, NoStore = true)]
    public class SetupController : Controller
    {
        [HttpPost, AllowAnonymous]
        public ActionResult Activate()
        {
            ConfigurationManager.RefreshSection("appSettings");
            if (ConfigurationManager.AppSettings["Configured"] != "true")
            {
                return RedirectToAction("Completed", new
                {
                    displayError = 1
                });
            }
            return Redirect("~/?#/welcome");
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
                    new FormUrlEncodedContent(new []
                    {
                        new KeyValuePair<string, string>("EmailAddress", model.Email),
                        new KeyValuePair<string, string>("CompanyName", model.CompanyName)
                    });
                await client.PostAsync("https://onetrueerror.com/support/register/", content);
                return Redirect(Url.GetNextWizardStep());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult Basics()
        {
            var model = new BasicsViewModel();
            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            if (config != null)
            {
                model.BaseUrl = config.BaseUrl.ToString();
                model.SupportEmail = config.SupportEmail;
            }
            else
            {
                model.BaseUrl = Request.Url.ToString().Replace("installation/setup/basics/", "").Replace("localhost", "yourServerName");
                ViewBag.NextLink = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Basics(BasicsViewModel model)
        {
            var settings = new BaseConfiguration();
            if (!model.BaseUrl.EndsWith("/"))
                model.BaseUrl += "/";

            if (model.BaseUrl.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) != -1)
            {
                ModelState.AddModelError("BaseUrl", "Use the servers real DNS name instead of 'localhost'. If you don't the Ajax request wont work as CORS would be enforced by IIS.");
                return View(model);
            }
            settings.BaseUrl = new Uri(model.BaseUrl);
            settings.SupportEmail = model.SupportEmail;
            ConfigurationStore.Instance.Store(settings);
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
            var config = ConfigurationStore.Instance.Load<OneTrueErrorConfigSection>();
            if (config != null)
            {
                model.ActivateTracking = config.ActivateTracking;
                model.ContactEmail = config.ContactEmail;
                model.InstallationId = config.InstallationId;
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

            var settings = new OneTrueErrorConfigSection();
            settings.ActivateTracking = model.ActivateTracking;
            settings.ContactEmail = model.ContactEmail;
            settings.InstallationId = model.InstallationId;
            ConfigurationStore.Instance.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }

        // GET: Installation/Home
        public ActionResult Index()
        {
            try
            {
                ConnectionFactory.Create();
            }
            catch
            {
                ViewBag.Ready = false;
            }
            return View();
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }
    }
}