using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using codeRR.Server.Api.Core.ApiKeys.Commands;
using codeRR.Server.Api.Core.ApiKeys.Queries;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.Web.Areas.Admin.Models.ApiKeys;
using codeRR.Server.Web.Controllers;
using DotNetCqs;

namespace codeRR.Server.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class ApiKeysController : Controller
    {
        private readonly IMessageBus _messageBus;
        private IQueryBus _queryBus;

        public ApiKeysController(IMessageBus messageBus, IQueryBus queryBus)
        {
            _messageBus = messageBus;
            _queryBus = queryBus;
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
            await _messageBus.SendAsync(this.ClaimsUser(), cmd);

            return RedirectToAction("Created", new { apiKey, sharedSecret });
        }

        public async Task<ActionResult> Edit(int id)
        {
            var applications = await GetMyApplications();
            var key = await _queryBus.QueryAsync(this.ClaimsUser(), new GetApiKey(id));
            var model = new EditViewModel
            {
                Id = key.Id,
                AvailableApplications = applications,
                SelectedApplications = Enumerable.Select(key.AllowedApplications, x => x.ApplicationId.ToString()).ToArray<string>(),
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

            await _messageBus.SendAsync(this.ClaimsUser(), cmd);

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
            await _messageBus.SendAsync(this.ClaimsUser(), cmd);
            return RedirectToAction("Deleted");
        }

        public ActionResult Deleted()
        {
            return View();
        }


        public async Task<ActionResult> Details(int id)
        {
            var key = await _queryBus.QueryAsync(this.ClaimsUser(), new GetApiKey(id));
            return View(key);
        }

        public async Task<ActionResult> Index()
        {
            var query = new ListApiKeys();
            var result = await _queryBus.QueryAsync(this.ClaimsUser(), query);
            var vms = Enumerable.Select(result.Keys, x => new ListViewModelItem { Id = x.Id, Name = x.ApplicationName }).ToArray<ListViewModelItem>();
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
            var items = await _queryBus.QueryAsync(this.ClaimsUser(), query);
            var applications = Enumerable.ToDictionary(items, x => x.Id.ToString(), x => x.Name);
            return applications;
        }
    }
}