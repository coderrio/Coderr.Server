using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using codeRR.Server.Api.Core.Applications.Queries;
using DotNetCqs;

namespace codeRR.Server.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IQueryBus _queryBus;

        public HomeController(IQueryBus queryBus1)
        {
            _queryBus = queryBus1;
        }

        public async Task<ActionResult> Index()
        {
            var apps = await _queryBus.QueryAsync(this.ClaimsUser(), new GetApplicationList());
            if (apps.Length == 0)
                return RedirectToAction("Application", "Wizard");

            if (!Request.Path.EndsWith("/"))
                return Redirect(Request.Path + "/");

            return View();
        }
    }
}