using System;
using System.Configuration;
using System.Web.Mvc;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Installation.Models;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
    public class SetupController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
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
                model.BaseUrl = Request.Url.ToString().Replace("installation/setup/basics/", "");
                ViewBag.NextLink = "";
            }


            return View(model);
        }

        [HttpPost]
        public ActionResult Basics(BasicsViewModel model)
        {
            var settings = new BaseConfiguration();
            settings.BaseUrl = new Uri(model.BaseUrl);
            settings.SupportEmail = model.SupportEmail;
            ConfigurationStore.Instance.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }

        [HttpPost]
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


        public ActionResult Completed(string displayError = null)
        {
            ViewBag.DisplayError = displayError == "1";
            return View();
        }
    }
}