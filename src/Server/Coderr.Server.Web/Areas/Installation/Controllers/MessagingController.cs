using System.Threading.Tasks;
using codeRR.Server.Web.Areas.Installation.Models;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.App.Modules.Messaging.Commands;
using Coderr.Server.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace codeRR.Server.Web.Areas.Installation.Controllers
{
    [Area("Installation")]
    
    public class MessagingController : Controller
    {
        private ConfigurationStore _configStore;

        public MessagingController(ConfigurationStore configStore)
        {
            _configStore = configStore;
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