using System;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using codeRR.Server.Api.Core.Accounts.Queries;
using codeRR.Server.Api.Core.Applications;
using codeRR.Server.Api.Core.Applications.Commands;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Api.Core.Incidents.Queries;
using codeRR.Server.Api.Core.Messaging;
using codeRR.Server.Api.Core.Messaging.Commands;
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
        private readonly ICommandBus _cmdBus;
        private readonly IQueryBus _queryBus;

        public WizardController(BaseConfiguration baseConfiguration, IQueryBus queryBus, ICommandBus cmdBus)
        {
            _baseConfiguration = baseConfiguration;
            _queryBus = queryBus;
            _cmdBus = cmdBus;
        }

        [Route("configure/application")]
        public async Task<ActionResult> Application()
        {
            await Init();
            return View();
        }

        [Route("configure/application")]
        [HttpPost]
        public async Task<ActionResult> Application(ApplicationViewModel model)
        {
            await Init();

            if (!ModelState.IsValid)
                return View(model);

            var app = new CreateApplication(model.Name, TypeOfApplication.DesktopApplication)
            {
                ApplicationKey = Guid.NewGuid().ToString("N"),
                UserId = User.GetAccountId()
            };
            await _cmdBus.ExecuteAsync(app);

            return RedirectToAction("Packages", new {appKey = app.ApplicationKey});
        }


        [Route("configure/package")]
        public async Task<ActionResult> ConfigurePackage(ConfigurePackageViewModel model, bool validationFailed = false)
        {
            await Init();
            var query = new GetApplicationInfo(model.ApplicationId);
            var appInfo = await _queryBus.QueryAsync(query);

            try
            {
                var hash = CalculateMd5Hash(appInfo.AppKey);

                // Fetch updates instructions from the git documentation
                // without having to manage it locally.
                var uri =
                    $"https://coderrapp.com/client/{model.LibraryName}/configure/{hash}";
                var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
                var html = await client.GetStringAsync(uri);
                model.Instruction = html
                    .Replace("yourAppKey", appInfo.AppKey)
                    .Replace("yourSharedSecret", appInfo.SharedSecret)
                    .Replace("http://yourServer/coderr/", _baseConfiguration.BaseUrl.ToString())
                    .Replace("\"/documentation/", "\"https://coderrapp.com/documentation/");
            }
            catch (Exception)
            {
                model.ErrorMessage = "Failed to download package information. Search nuget.org after the correct package (starting with 'coderr.client')";
            }

            model.SharedSecret = appInfo.SharedSecret;
            model.AppKey = appInfo.AppKey;
            model.ReportUrl = _baseConfiguration.BaseUrl;
            return View(model);
        }
        
        [Route("configure/choose/package")]
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
                var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
                var clientsJSON = await client.GetStringAsync("https://coderrapp.com/configure/packages/");
                model.Packages = JsonConvert.DeserializeObject<NugetPackage[]>(clientsJSON);
            }
            catch (Exception)
            {
                model.ErrorMessage = "Failed to download package information";
            }

            return View(model);
        }

        [Route("configure/choose/package")]
        [HttpPost]
        public async Task<ActionResult> Packages(PackagesViewModel model)
        {
            await Init();
            try
            {
                var client = new HttpClient {Timeout = TimeSpan.FromSeconds(10)};
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

            return RedirectToAction("ConfigurePackage", new {model.LibraryName, model.ApplicationId});
        }

        [Route("validate/configuration")]
        public async Task<ActionResult> Validate(ValidateViewModel model)
        {
            var query = new FindIncidents
            {
                ApplicationId = model.ApplicationId
            };
            var result = await _queryBus.QueryAsync(query);
            if (result.TotalCount == 0)
                return View(model);

            return Redirect("~/#/application/" + model.ApplicationId);
        }

        protected async Task Init(int maxCount = 0)
        {
            var query = new GetApplicationList();
            var result = await _queryBus.QueryAsync(query);
            ViewBag.FirstApplication = result.Length <= maxCount;
        }

        private string CalculateMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("x2"));
            return sb.ToString();
        }
    }
}