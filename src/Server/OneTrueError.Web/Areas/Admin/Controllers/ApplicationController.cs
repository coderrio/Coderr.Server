using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCqs;
using OneTrueError.Api.Core.Applications.Commands;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Web.Areas.Admin.Models.Applications;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    public class ApplicationController : Controller
    {
        private IQueryBus _queryBus;
        private ICommandBus _cmdBus;


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
            SessionUser.Current.Applications.Remove(id);
            return RedirectToAction("Deleted");
        }

        public ActionResult Deleted()
        {
            return View();
        }

        public async Task<ActionResult> Index()
        {
            var apps = await _queryBus.QueryAsync(new GetApplicationList());
            var model = apps.Select(x => new ApplicationViewModel {Id = x.Id, Name = x.Name}).ToList();
            return View(model);
        }
    }
}