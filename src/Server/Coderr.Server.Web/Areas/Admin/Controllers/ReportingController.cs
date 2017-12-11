using System.Web.Mvc;
using codeRR.Server.App.Core.Reports.Config;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Web.Areas.Admin.Models;
using Coderr.Server.PluginApi.Config;
using log4net;

namespace codeRR.Server.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ReportingController : Controller
    {
        private ILog _logger = LogManager.GetLogger(typeof(ReportingController));
        private ConfigurationStore _configStore;

        public ReportingController(ConfigurationStore configStore)
        {
            _configStore = configStore;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var model = new ReportingViewModel();
            var settings = _configStore.Load<ReportConfig>();
            if (settings == null || settings.MaxReportsPerIncident == 0)
                return View(model);

            _logger.Debug("Display acess: " + settings.MaxReportsPerIncident + " from " + _configStore.GetHashCode());
            model.MaxReportsPerIncident = settings.MaxReportsPerIncident;
            model.RetentionDays= settings.RetentionDays;
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(ReportingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var settings = new ReportConfig
            {
                MaxReportsPerIncident = model.MaxReportsPerIncident,
                RetentionDays = model.RetentionDays,
            };
            _logger.Debug("Storing: " + settings.MaxReportsPerIncident);
            _configStore.Store(settings);
            _logger.Debug("Stored: " + settings.MaxReportsPerIncident + " to " + _configStore.GetHashCode());

            return RedirectToAction("Index", "Home");
        }
    }
}