using System;
using System.Threading.Tasks;
using Coderr.Client;
using Coderr.Server.Live.LobbyIntegration.Authentication.SingleSignOn;
using Coderr.Server.Web.Boot;
using Coderr.Server.Web.Boot.Adapters;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Coderr.Server.Web.Controllers
{
    public class LobbyController : Controller
    {
        private IConfiguration _configuration;
        private ILog _logger = LogManager.GetLogger(typeof(LobbyController));

        public LobbyController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Go to the login page in Lobby
        /// </summary>
        /// <returns></returns>
        public IActionResult ToLobby()
        {
            var lobbyUrl = _configuration["Live/Lobby"];
            return Redirect(lobbyUrl + "account/login/");
        }

        public async Task<IActionResult> Login()
        {
            var options = new SingleSignOnAuthenticationOptions
            {
                OpenGlobalDb = () => LiveStart.OpenGlobalConnection(new ConfigurationWrapper(_configuration)),
                OpenTenantDb = orgId => LiveStart.OpenTenantDb(new ConfigurationWrapper(_configuration), orgId),
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme
            };

            _logger.Debug("Creating ticket");
            try
            {
                var ticket =
                    await SingleSignOnAuthenticationHandler.CreateAuthenticationTicket(Request, options, "token");
                if (ticket == null)
                {
                    _logger.Error("Not authorized.");
                    return Unauthorized();
                }


                _logger.Debug("logging in");
                await HttpContext.SignInAsync(ticket.Principal);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to create login ticket.", ex);
                Err.Report(ex);
            }



            //var identity = new ClaimsIdentity(new List<Claim>
            //{
            //    new Claim(ClaimTypes.NameIdentifier, "1"),
            //    new Claim(ClaimTypes.Name, "Arne"),
            //    new Claim(LiveClaims.OrganizationId, "23")
            //}, CookieAuthenticationDefaults.AuthenticationScheme);
            //
            //var options = new AuthenticationProperties
            //{
            //    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14),
            //    IsPersistent = true
            //};
            HttpContext.Items["IgnoreTransaction"] = true;


            _logger.Debug("Logged in");
            return Redirect("/");
        }
    }
}
