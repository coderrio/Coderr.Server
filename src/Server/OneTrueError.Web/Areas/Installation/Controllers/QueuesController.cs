using System.Web.Mvc;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Infrastructure.Queueing;
using OneTrueError.Web.Areas.Installation.Models;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
    public class QueuesController : Controller
    {
        public ActionResult Index()
        {
            var model = new QueueViewModel();

            var settings = ConfigurationStore.Instance.Load<MessageQueueSettings>();
            if (settings == null)
            {
                ViewBag.NextLink = null;
                model.UseSql = true;
                return View(model);
            }

            model.UseSql = true;
            model.EventQueue = settings.EventQueue;
            model.EventAuthentication = settings.EventAuthentication;
            model.EventTransactions = settings.EventTransactions;
            model.EventQueue = settings.ReportQueue;
            model.ReportTransactions = settings.ReportTransactions;
            model.ReportAuthentication = settings.ReportAuthentication;
            model.FeedbackQueue = settings.FeedbackQueue;
            model.FeedbackAuthentication = settings.FeedbackAuthentication;
            model.FeedbackTransactions = settings.FeedbackTransactions;

            return View(model);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }


        [HttpPost]
        public ActionResult Index(QueueViewModel model)
        {
            var config = new MessageQueueSettings();
            if (model.UseSql)
            {
                config.UseSql = true;
                ConfigurationStore.Instance.Store(config);
                return Redirect(Url.GetNextWizardStep());
            }

            var errorMessage = "";
            if (
                !SetupTools.ValidateMessageQueue(model.ReportQueue, model.ReportAuthentication, model.ReportTransactions, out errorMessage))
            {
                ModelState.AddModelError("ReportQueue", errorMessage);
            }
            if (
                !SetupTools.ValidateMessageQueue(model.FeedbackQueue, model.FeedbackAuthentication, model.FeedbackTransactions, out errorMessage))
            {
                ModelState.AddModelError("FeedbackQueue", errorMessage);
            }
            if (
                !SetupTools.ValidateMessageQueue(model.EventQueue, model.EventAuthentication, model.EventTransactions, out errorMessage))
            {
                ModelState.AddModelError("EventQueue", errorMessage);
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            config.EventQueue = model.EventQueue;
            config.EventAuthentication = model.EventAuthentication;
            config.EventTransactions = model.EventTransactions;
            config.EventQueue = model.ReportQueue;
            config.ReportTransactions = model.ReportTransactions;
            config.ReportAuthentication = model.ReportAuthentication;
            config.FeedbackQueue = model.FeedbackQueue;
            config.FeedbackAuthentication = model.FeedbackAuthentication;
            config.FeedbackTransactions = model.FeedbackTransactions;
            ConfigurationStore.Instance.Store(config);

            return Redirect(Url.GetNextWizardStep());
        }
    }
}