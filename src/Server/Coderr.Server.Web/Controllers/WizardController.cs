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
using Griffin.Data;
using Newtonsoft.Json;

namespace codeRR.Server.Web.Controllers
{
    [Authorize]
    public class WizardController : Controller
    {
        private readonly BaseConfiguration _baseConfiguration;
        private readonly IMessageBus _messageBus;
        private IQueryBus _queryBus;

        public WizardController(BaseConfiguration baseConfiguration, IMessageBus messageBus, IQueryBus queryBus)
        {
            _baseConfiguration = baseConfiguration;
            _messageBus = messageBus;
            _queryBus = queryBus;
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
            await _messageBus.SendAsync(this.ClaimsUser(), app);
            await Task.Delay(100);//ugly!

            var returnUrl = Url.Action("Packages", new {appKey = app.ApplicationKey});
            return RedirectToAction("UpdateSession", "Account", new {returnUrl});
        }


        [Route("configure/package")]
        public async Task<ActionResult> ConfigurePackage(ConfigurePackageViewModel model, bool validationFailed = false)
        {
            await Init();
            var query = new GetApplicationInfo(model.ApplicationId);
            var appInfo = await _queryBus.QueryAsync(this.ClaimsUser(), query);

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
                try
                {
                    var appInfo = await _queryBus.QueryAsync(this.ClaimsUser(), new GetApplicationInfo(appKey));
                    applicationId = appInfo.Id;
                }
                catch (EntityNotFoundException)
                {
                    await Task.Delay(1000);
                    return RedirectToAction("Packages", new {appKey});
                }
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
                ApplicationIds = new []{model.ApplicationId}
            };
            var result = await _queryBus.QueryAsync(this.ClaimsUser(), query);
            if (result.TotalCount == 0)
                return View(model);

            return Redirect("~/#/application/" + model.ApplicationId + "/");
        }

        protected async Task Init(int maxCount = 0)
        {
            var query = new GetApplicationList();
            var result = await _queryBus.QueryAsync(this.ClaimsUser(), query);
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