using System.Web.Mvc;
using codeRR.Server.App.Modules.Messaging.Commands;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Web.Areas.Installation.Models;
using Coderr.Server.PluginApi.Config;

namespace codeRR.Server.Web.Areas.Installation.Controllers
{
    [OutputCache(Duration = 0, NoStore = true)]
    public class MessagingController : Controller
    {
        private ConfigurationStore _configStore;

        public MessagingController()
        {
            _configStore = Startup.ConfigurationStore;
        }

        public ActionResult Email()
        {
            var model = new EmailViewModel();
            var settings = _configStore.Load<DotNetSmtpSettings>();
            if (!string.IsNullOrEmpty(settings.SmtpHost))
            {
                model.AccountName = settings.AccountName;
                model.PortNumber = settings.PortNumber;
                model.SmtpHost = settings.SmtpHost;
                model.UseSSL = settings.UseSsl;
                model.AccountPassword = settings.AccountPassword;
            }
            else
            {
                ViewBag.NextLink = "";
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Email(EmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var settings = new DotNetSmtpSettings
            {
                AccountName = model.AccountName,
                PortNumber = model.PortNumber ?? 25,
                AccountPassword = model.AccountPassword,
                SmtpHost = model.SmtpHost,
                UseSsl = model.UseSSL
            };
            _configStore.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }
    }
}