using System;
using System.Threading.Tasks;
using Coderr.Server.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

namespace Coderr.Server.Web.Areas.Installation.Controllers
{
    [Area("Installation")]
    
    public class SqlController : Controller
    {
        private static int _counter;
        private IConfiguration _config;

        public SqlController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        public ActionResult Connection()
        {
            return RedirectToAction("Tables");
        }

        public ActionResult Index()
        {
            var constr = _config.GetConnectionString("Db");
            if (!string.IsNullOrEmpty(constr))
            {
                ViewBag.ConnectionString = constr ?? "";
            }
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
            {
                ViewBag.GotTables = true;
            }
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
                var constr = _config.GetConnectionString("Db");
                constr = ChangeConnectionTimeout(constr);
                SetupTools.DbTools.TestConnection(constr);
                return Content(@"{ ""result"": ""ok"" }", "application/json");
            }
            catch (Exception ex)
            {
                var errMsg = ex.Message.Replace("\\", "\\\\")
                    .Replace("\"", "\\\"")
                    .Replace("\r", "")
                    .Replace("\n", "\\n");
                return Json(new
                {
                    result = "fail",
                    reason = errMsg,
                    attempt = ++_counter
                });
            }
        }
        internal static string ChangeConnectionTimeout(string conStr)
        {
            var pos = conStr.IndexOf("Connect Timeout", StringComparison.OrdinalIgnoreCase);
            if (pos != -1)
            {
                var pos2 = conStr.IndexOf(';', pos);
                if (pos2 == -1)
                {
                    conStr = conStr.Substring(0, pos) + "Connect Timeout=5";
                }
                else
                {
                    conStr = conStr.Substring(0, pos) + "Connect Timeout=5;" + conStr.Substring(pos2);
                }
            }

            return conStr;
        }
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            return base.OnActionExecutionAsync(context, next);
        }
    }
}