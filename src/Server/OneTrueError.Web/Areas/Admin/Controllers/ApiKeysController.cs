using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DotNetCqs;
using OneTrueError.Api.Core.ApiKeys.Commands;
using OneTrueError.Api.Core.ApiKeys.Queries;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.App.Core.Applications;
using OneTrueError.Web.Areas.Admin.Models.ApiKeys;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Areas.Admin.Controllers
{
    public class ApiKeysController : Controller
    {
        private IQueryBus _queryBus;
        private ICommandBus _commandBus;
        public ApiKeysController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        public async Task<ActionResult> Index()
        {
            var query = new ListApiKeys();
            var result = await _queryBus.QueryAsync(query);
            var vms = result.Keys.Select(x => new ListViewModelItem { Id = x.Id, Name = x.ApplicationName }).ToArray();
            var model = new ListViewModel { Keys = vms };
            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var cmd = new DeleteApiKey(id);
            await _commandBus.ExecuteAsync(cmd);
            return RedirectToAction("Deleted");
        }

        public ActionResult Deleted()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var applications = await GetMyApplications();

            var model = new CreateViewModel { Applications = applications };
            return View(model);
        }

        private async Task<Dictionary<string, string>> GetMyApplications()
        {
            var query = new GetApplicationList
            {
                AccountId = SessionUser.Current.AccountId,
                FilterAsAdmin = true
            };
            var items = await _queryBus.QueryAsync(query);
            var applications = items.ToDictionary(x => x.Id.ToString(), x => x.Name);
            return applications;
        }


        public async Task<ActionResult> Details(int id)
        {
            var key = await _queryBus.QueryAsync(new GetApiKey(id));
            return View(key);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            model.Applications = await GetMyApplications();
            if (!ModelState.IsValid)
                return View(model);

            var apiKey = Guid.NewGuid().ToString("N");
            var sharedSecret = Guid.NewGuid().ToString("N");
            var apps = model.SelectedApplications == null
                ? new int[0]
                : model.SelectedApplications.Select(int.Parse).ToArray();
            var cmd = new CreateApiKey(model.ApplicationName, apiKey, sharedSecret, apps);
            await _commandBus.ExecuteAsync(cmd);

            return RedirectToAction("Created", new { apiKey, sharedSecret });
        }

        public ActionResult Created(string apiKey, string sharedSecret)
        {
            ViewBag.ApiKey = apiKey;
            ViewBag.SharedSecret = sharedSecret;
            return View();
        }

    }
}