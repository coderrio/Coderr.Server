using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetCqs;
using OneTrueError.Api.Core.Applications.Commands;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Api.Modules.Triggers.Queries;
using OneTrueError.App.Modules.Versions.Config;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Areas.Admin.Models.Applications;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
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

        public async Task<ActionResult> Versions(int id)
        {
            var items = await FetchAssemblies(id);
            var model = new ApplicationVersionViewModel
            {
                ApplicationId = id,
                Assemblies = items
            };

            //null if this is the first time we run or if this specific application have not been configured yet.
            var item = ConfigurationStore.Instance.Load<ApplicationVersionConfig>();
            var myAssembly = item?.Items.FirstOrDefault(x => x.ApplicationId == id);
            model.SelectedAssembly = myAssembly?.AssemblyName;

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Versions(ApplicationVersionViewModel model)
        {
            model.Assemblies = await FetchAssemblies(model.ApplicationId);
            if (!ModelState.IsValid)
                return View(model);

            //null if this is the first time we run
            var config = ConfigurationStore.Instance.Load<ApplicationVersionConfig>() ?? new ApplicationVersionConfig();
            config.AddOrUpdate(model.ApplicationId, model.SelectedAssembly);
            ConfigurationStore.Instance.Store(config);

            return RedirectToAction("Index", new {usernote = "Assembly was updated successfully."});
        }

        private async Task<SelectListItem[]> FetchAssemblies(int id)
        {
            var query = new GetContextCollectionMetadata(id);
            var assemblyReslt = await _queryBus.QueryAsync(query);
            var assemblyCollection = assemblyReslt.FirstOrDefault(x => x.Name == "Assemblies");
            var items = new SelectListItem[0];
            if (assemblyCollection != null)
            {
                items = (
                    from x in assemblyCollection.Properties
                    where !x.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                          && !x.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                    select new SelectListItem {Value = x, Text = x}
                ).ToArray();
            }
            return items;
        }
    }
}