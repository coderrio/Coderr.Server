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

        public HomeController(IQueryBus queryBus)
        {
            _queryBus = queryBus ?? throw new ArgumentNullException(nameof(queryBus));
        }

        public async Task<ActionResult> Index()
        {
            var apps = await _queryBus.QueryAsync(new GetApplicationList());
            if (apps.Length == 0)
                return RedirectToAction("Application", "Wizard");

            return View();
        }

 

    }
}