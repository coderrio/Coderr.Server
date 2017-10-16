using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using codeRR.Server.Api.Core.Applications;
using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Api.Core.Incidents.Queries;
using codeRR.Server.App.Configuration;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.Web.Models.Wizard;
using DotNetCqs;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Controllers
{
    [Authorize]
    public class WizardController : Controller
    {
        private readonly BaseConfiguration _baseConfiguration;
        private readonly IQueryBus _queryBus;
        private ICommandBus _cmdBus;

        public WizardController(BaseConfiguration baseConfiguration, IQueryBus queryBus, ICommandBus cmdBus)
        {
            _baseConfiguration = baseConfiguration;
            _queryBus = queryBus;
            _cmdBus = cmdBus;
        }

        public async Task<ActionResult> Application()
        {
            await Init();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Application(ApplicationViewModel model)
        {
            await Init();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var app = new CreateApplication(model.Name, TypeOfApplication.DesktopApplication)
            {
                ApplicationKey = Guid.NewGuid().ToString("N"),
                UserId = User.GetAccountId()
            };
            await _cmdBus.ExecuteAsync(app);

            return RedirectToAction("Packages", new {appKey = app.ApplicationKey});
        }


        public async Task<ActionResult> Packages(int? applicationId, string appKey)
        {
            await Init();

            if (applicationId == null)
            {
                var appInfo = await _queryBus.QueryAsync(new GetApplicationInfo(appKey));
                applicationId = appInfo.Id;
            }

            var model = new PackagesViewModel {ApplicationId = applicationId.Value};

            try
            {
                var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var clientsJSON = await client.GetStringAsync("https://coderrapp.com/configure/packages/");
                model.Packages = JsonConvert.DeserializeObject<NugetPackage[]>(clientsJSON);
            }
            catch (Exception)
            {
                model.ErrorMessage = "Failed to download package information";
            }

            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Packages(PackagesViewModel model)
        {
            await Init();
            try
            {
                var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var clientsJSON = await client.GetStringAsync("https://coderrapp.com/configure/packages/");
                model.Packages = JsonConvert.DeserializeObject<NugetPackage[]>(clientsJSON);
            }
            catch (Exception)
            {
                model.ErrorMessage = "Failed to download package information";
            }

            if (string.IsNullOrEmpty(model.LibraryName))
            {
                ModelState.AddModelError("LibraryName", "A package must be selected");
                return View(model);
            }

            return RedirectToAction("ConfigurePackage", new { model.LibraryName, model.ApplicationId });
        }



        public async Task<ActionResult> ConfigurePackage(ConfigurePackageViewModel model, bool validationFailed = false)
        {
            await Init();
            var query = new GetApplicationInfo(model.ApplicationId);
            var appInfo = await _queryBus.QueryAsync(query);

            try
            {
                var uri =
                    $"https://coderrapp.com/client/{model.LibraryName}/configure/?appKey={appInfo.AppKey}&sharedSecret={appInfo.SharedSecret}&hostName={_baseConfiguration.BaseUrl}";
                var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                var html = await client.GetStringAsync(uri);
                model.Instruction = html;
            }
            catch (Exception)
            {
                model.ErrorMessage = "Failed to download package information";
            }

            model.SharedSecret = appInfo.SharedSecret;
            model.AppKey = appInfo.AppKey;
            model.ReportUrl = _baseConfiguration.BaseUrl;
            return View(model);
        }

        public async Task<ActionResult> Validate(ValidateViewModel model)
        {
            var query = new FindIncidents()
            {
                ApplicationId = model.ApplicationId
            };
            var result = await _queryBus.QueryAsync(query);
            if (result.TotalCount == 0)
            {
                return View();
            }

            return Redirect("~/#/application/" + model.ApplicationId);
        }

        protected async Task Init(int maxCount = 0)
        {
            var query = new GetApplicationList();
            var result = await _queryBus.QueryAsync(query);
            ViewBag.FirstApplication = result.Length <= maxCount;
        }

    }
}