using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetCqs;
using OneTrueError.Api.Core.Applications.Commands;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Web.Areas.Admin.Models.Applications;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly ICommandBus _cmdBus;
        private readonly IQueryBus _queryBus;


        public ApplicationController(IQueryBus queryBus, ICommandBus cmdBus)
        {
            if (queryBus == null) throw new ArgumentNullException("queryBus");
            if (cmdBus == null) throw new ArgumentNullException("cmdBus");

            _queryBus = queryBus;
            _cmdBus = cmdBus;
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var cmd = new DeleteApplication(id);
            await _cmdBus.ExecuteAsync(cmd);
            await Task.Delay(500);

            var url = Url.Action("Deleted");
            return RedirectToAction("UpdateSession", "Account", new {area = "", returnUrl = url});
        }

        public ActionResult Deleted()
        {
            return View();
        }

        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetApplicationInfo(id);
            var app = await _queryBus.QueryAsync(query);
            var model = new EditViewModel
            {
                ApplicationId = app.Id,
                Name = app.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cmd = new UpdateApplication(model.ApplicationId, model.Name);
            await _cmdBus.ExecuteAsync(cmd);

            var dict = new RouteValueDictionary {{"usernote", "Application was updated"}};
            return RedirectToAction("Index", dict);
        }

        public async Task<ActionResult> Index()
        {
            var apps = await _queryBus.QueryAsync(new GetApplicationList());
            var model = apps.Select(x => new ApplicationViewModel {Id = x.Id, Name = x.Name}).ToList();
            return View(model);
        }
    }
}