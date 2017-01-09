using System.Web.Mvc;
using OneTrueError.App.Modules.Messaging.Commands;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Admin.Models;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    public class MessagingController : Controller
    {
        public ActionResult Email()
        {
            var model = new EmailViewModel();
            var settings = ConfigurationStore.Instance.Load<DotNetSmtpSettings>();
            if (settings == null || string.IsNullOrEmpty(settings.SmtpHost))
                return View(model);

            model.AccountName = settings.AccountName;
            model.PortNumber = settings.PortNumber;
            model.SmtpHost = settings.SmtpHost;
            model.UseSSL = settings.UseSsl;
            model.AccountPassword = settings.AccountPassword;
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
            ConfigurationStore.Instance.Store(settings);
            return Redirect(Url.GetNextWizardStep());
        }
    }
}