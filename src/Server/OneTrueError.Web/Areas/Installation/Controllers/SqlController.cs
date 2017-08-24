using System;
using System.Configuration;
using System.Web.Mvc;
using Griffin.Data;
using OneTrueError.App.Core.Applications;
using OneTrueError.Infrastructure;
using OneTrueError.SqlServer.Core.Applications;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
    public class SqlController : Controller
    {
        [HttpPost]
        public ActionResult Connection()
        {
            return RedirectToAction("Tables");
        }

        public ActionResult Index()
        {
            var constr = ConfigurationManager.ConnectionStrings["Db"];
            if (!string.IsNullOrEmpty(constr?.ConnectionString))
                ViewBag.ConnectionString = constr.ConnectionString ?? "";
            else
            {
                ViewBag.ConnectionString = "";
                ViewBag.NextLink = "";
            }


            return View();
        }

        [HttpGet]
        public ActionResult Tables()
        {
            ViewBag.GotException = false;
            if (SetupTools.DbTools.GotUpToDateTables())
                ViewBag.GotTables = true;
            else
            {
                ViewBag.GotTables = false;
                ViewBag.NextLink = "";
            }
            return View();
        }

        [HttpPost]
        public ActionResult Tables(string go)
        {
            try
            {
                SetupTools.DbTools.CreateTables();
                SetupTools.DbTools.UpgradeDatabaseSchema();
                return Redirect(Url.GetNextWizardStep());
            }
            catch (Exception ex)
            {
                ViewBag.GotException = true;
                ViewBag.GotTables = false;
                ModelState.AddModelError("", ex.Message);
                ViewBag.FullException = ex.ToString();
                return View();
            }
            //return RedirectToRoute(new {Area = "Installation", Controller = "Setup", Action = "Done"});
        }

        public ActionResult Validate()
        {
            try
            {
                var constr = ConfigurationManager.ConnectionStrings["Db"];
                SetupTools.DbTools.CheckConnectionString(constr?.ConnectionString);
                return Content(@"{ ""result"": ""ok"" }", "application/json");
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "")
                    .Replace("\n", "\\n");
                return Content(@"{ ""result"": ""fail"", ""reason"": """ + errMsg + @"""}", "application/json");
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }
    }
}