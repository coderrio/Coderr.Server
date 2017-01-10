using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using OneTrueError.App.Core.Reports.Config;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Admin.Models;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    public class ReportingController : Controller
    {
        private ILog _logger = LogManager.GetLogger(typeof(ReportingController));

        [HttpGet]
        public ActionResult Index()
        {
            var model = new ReportingViewModel();
            var settings = ConfigurationStore.Instance.Load<ReportConfig>();
            if (settings == null || settings.MaxReportsPerIncident == 0)
                return View(model);

            _logger.Debug("Display acess: " + settings.MaxReportsPerIncident + " from " + ConfigurationStore.Instance.GetHashCode());
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
            ConfigurationStore.Instance.Store(settings);
            _logger.Debug("Stored: " + settings.MaxReportsPerIncident + " to " + ConfigurationStore.Instance.GetHashCode());

            return RedirectToAction("Index", "Home");
        }
    }
}