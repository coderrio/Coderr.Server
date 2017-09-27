using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCqs;
using codeRR.Api.Core.ApiKeys.Commands;
using codeRR.Api.Core.ApiKeys.Queries;
using codeRR.Api.Core.Applications.Queries;
using codeRR.Infrastructure.Security;
using codeRR.Web.Areas.Admin.Models.ApiKeys;

namespace codeRR.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ApiKeysController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;

        public ApiKeysController(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        public async Task<ActionResult> Create()
        {
            var applications = await GetMyApplications();

            var model = new CreateViewModel { AvailableApplications = applications };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            model.AvailableApplications = await GetMyApplications();
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

        public async Task<ActionResult> Edit(int id)
        {
            var applications = await GetMyApplications();
            var key = await _queryBus.QueryAsync(new GetApiKey(id));
            var model = new EditViewModel
            {
                Id = key.Id,
                AvailableApplications = applications,
                SelectedApplications = key.AllowedApplications.Select(x => x.ApplicationId.ToString()).ToArray(),
                ApplicationName = key.ApplicationName
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditViewModel model)
        {
            model.AvailableApplications = await GetMyApplications();
            if (!ModelState.IsValid)
                return View(model);

            var cmd = new EditApiKey(model.Id)
            {
                ApplicationIds = model.SelectedApplications == null
                    ? new int[0]
                    : model.SelectedApplications.Select(int.Parse).ToArray(),
                ApplicationName = model.ApplicationName
            };

            await _commandBus.ExecuteAsync(cmd);

            return RedirectToAction("Details", new { model.Id });
        }


        public ActionResult Created(string apiKey, string sharedSecret)
        {
            ViewBag.ApiKey = apiKey;
            ViewBag.SharedSecret = sharedSecret;
            return View();
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


        public async Task<ActionResult> Details(int id)
        {
            var key = await _queryBus.QueryAsync(new GetApiKey(id));
            return View(key);
        }

        public async Task<ActionResult> Index()
        {
            var query = new ListApiKeys();
            var result = await _queryBus.QueryAsync(query);
            var vms = result.Keys.Select(x => new ListViewModelItem { Id = x.Id, Name = x.ApplicationName }).ToArray();
            var model = new ListViewModel { Keys = vms };
            return View(model);
        }

        private async Task<Dictionary<string, string>> GetMyApplications()
        {
            var query = new GetApplicationList
            {
                AccountId = User.GetAccountId(),
                FilterAsAdmin = true
            };
            var items = await _queryBus.QueryAsync(query);
            var applications = items.ToDictionary(x => x.Id.ToString(), x => x.Name);
            return applications;
        }
    }
}