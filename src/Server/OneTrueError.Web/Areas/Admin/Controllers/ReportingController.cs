using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OneTrueError.App.Core.Reports.Config;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Admin.Models;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    public class ReportingController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var model = new ReportingViewModel();
            var settings = ConfigurationStore.Instance.Load<ReportConfig>();
            if (settings == null || settings.MaxReportsPerIncident == 0)
                return View(model);

            model.MaxReportsPerIncident = settings.MaxReportsPerIncident;
            model.RetentionDays= settings.RetentionDays;
            return View();
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
            ConfigurationStore.Instance.Store(settings);

            return View();
        }
    }
}