using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Api.Modules.Triggers.Queries;
using codeRR.Server.Web.Areas.Admin.Models.Applications;
using codeRR.Server.Web.Controllers;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;

namespace codeRR.Server.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ApplicationController : Controller
    {
        private readonly IMessageBus _messageBus;
        private IQueryBus _queryBus;
        private ConfigurationStore _configStore;

        public ApplicationController(IMessageBus messageBus, IQueryBus queryBus, ConfigurationStore configStore)
        {
            if (messageBus == null) throw new ArgumentNullException("messageBus");

            _messageBus = messageBus;
            _queryBus = queryBus;
            _configStore = configStore;
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var cmd = new DeleteApplication(id);
            await _messageBus.SendAsync(this.ClaimsUser(), cmd);
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
            var app = await _queryBus.QueryAsync(this.ClaimsUser(), query);
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
            await _messageBus.SendAsync(this.ClaimsUser(), cmd);

            var dict = new RouteValueDictionary {{"usernote", "Application was updated"}};
            return RedirectToAction("Index", dict);
        }

        public async Task<ActionResult> Index()
        {
            var apps = await _queryBus.QueryAsync(this.ClaimsUser(), new GetApplicationList());
            var model = Enumerable.Select(apps, x => new ApplicationViewModel {Id = x.Id, Name = x.Name}).ToList<ApplicationViewModel>();
            return View(model);
        }
        
        private async Task<SelectListItem[]> FetchAssemblies(int id)
        {
            var query = new GetContextCollectionMetadata(id);
            var assemblyReslt = await _queryBus.QueryAsync(this.ClaimsUser(), query);
            var assemblyCollection = Enumerable.FirstOrDefault(assemblyReslt, x => x.Name == "Assemblies");
            var items = new SelectListItem[0];
            if (assemblyCollection != null)
            {
                items = Enumerable.ToArray<SelectListItem>((
                    from x in assemblyCollection.Properties
                    where !x.StartsWith("System.", StringComparison.OrdinalIgnoreCase)
                          && !x.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)
                    select new SelectListItem {Value = x, Text = x}
                ));
            }
            return items;
        }
    }
}