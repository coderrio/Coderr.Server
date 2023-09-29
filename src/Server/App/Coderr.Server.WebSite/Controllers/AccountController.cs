using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Config;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Accounts.Commands;
using Coderr.Server.Api.Core.Accounts.Requests;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Api.Core.Invitations.Queries;
using Coderr.Server.App.Core.Accounts;
using Coderr.Server.Infrastructure.Configuration;
using Coderr.Server.WebSite.Infrastructure;
using Coderr.Server.WebSite.Models.Accounts;
using DotNetCqs;
using Griffin.Data;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Coderr.Server.WebSite.Controllers
{
    /// <summary>
    ///     TODO: Break out logic
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAdoNetUnitOfWork _uow;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
        private readonly IMessageBus _messageBus;
        private readonly IQueryBus _queryBus;
        private readonly ConfigurationStore _configStore;

        public AccountController(IAccountService accountService, IMessageBus messageBus, IAdoNetUnitOfWork uow, IQueryBus queryBus, ConfigurationStore configStore)
        {
            _accountService = accountService;
            _messageBus = messageBus;
            _uow = uow;
            _queryBus = queryBus;
            _configStore = configStore;
        }

        /// <summary>
        ///     Accept invitation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/account/accept/{id}")]
        public async Task<ActionResult> Accept(string id)
        {
            try
            {
                var query = new GetInvitationByKey(id);
                var invitation = await _queryBus.QueryAsync(query);
                if (invitation == null)
                    return View("InvitationNotFound");

                var model = new AcceptViewModel
                {
                    InvitationKey = id,
                    Email = invitation.EmailAddress
                };

                return View(model);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
                _logger.Error("Failed ot launch", exception);
                return View(new AcceptViewModel());
            }
        }

        [HttpPost("api/account/accept")]
        public async Task<LoginResult> Accept(AcceptViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }

            var cmd = new AcceptInvitation(model.UserName, model.Password, model.InvitationKey)
            {
                AcceptedEmail = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var identity = await _accountService.AcceptInvitation(User, cmd);

            //TODO: Remove hack.
            // HERE since the message queue starts to process the events
            // before we are done with them. We need some way to stack up the publishing
            // until the current handler is done.
            //
            // can't use a message handler since we need a result from the invitation accept.
            // so that we can construct a new identity
            _uow.SaveChanges();

            if (identity == null)
            {
                ModelState.AddModelError("",
                    "Failed to find an invitation with the specified key. You might have already accepted the invitation? If not, ask for a new one.");
                _logger.Error("Failed to find invitation " + model.InvitationKey);
                return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }


            var token = JwtHelper.GenerateToken(identity);
            return new LoginResult { Success = true, JwtToken = token };
        }

        [HttpPost("api/account/activate/{id}")]
        public async Task<LoginResult> Activate(string id, string returnUrl = null)
        {
            try
            {
                _logger.Debug("Activating " + id);
                var identity = await _accountService.ActivateAccount(User, id);
                _logger.Debug("Signin " + id);
                var token = JwtHelper.GenerateToken(identity);
                return new LoginResult { Success = true, JwtToken = token };

            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.Warn("Failed to activate using " + id, ex);
                ModelState.AddModelError("", "Activation key was not found.");
            }
            catch (Exception err)
            {
                _logger.Warn("Failed to activate using " + id, err);
                ModelState.AddModelError("", err.Message);
            }

            return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
        }

        [HttpGet("account/activation/requested")]
        public ActionResult ActivationRequested()
        {
            return View();
        }

        [HttpPost("api/account/login")]
        public async Task<LoginResult> Login([FromBody] LoginViewModel model)
        {
            var config = _configStore.Load<BaseConfiguration>();
            model.AllowRegistrations = config.AllowRegistrations != false;

            if (!ModelState.IsValid)
            {
                return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }

            try
            {
                var identity = await _accountService.Login(model.UserName, model.Password);
                if (identity == null)
                {
                    ModelState.AddModelError("", "Incorrect username or password.");
                    model.Password = "";
                    return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
                }
                var token = JwtHelper.GenerateToken(identity);
                return new LoginResult { Success = true, JwtToken = token };
            }
            catch (AuthenticationException err)
            {
                _logger.Error("Failed to authenticate", err);
                ModelState.AddModelError("", err.Message);
                return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to authenticate", exception);
                ModelState.AddModelError("", "Failed to authenticate");
                return new LoginResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }
        }


        [HttpGet("api/account/logout")]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (ServerConfig.Instance.IsLive)
                return Redirect("https://lobby.coderr.io/");

            return Redirect("~/");
        }

        [HttpGet("api/account/register")]
        public ActionResult Register()
        {
            var model = new RegisterViewModel { ReturnUrl = Request.Query["ReturnUrl"].FirstOrDefault() };
            return View(model);
        }


        [HttpPost("api/account/register")]
        public async Task<RegisterResult> Register([FromBody]RegisterViewModel model)
        {
            var config = _configStore.Load<BaseConfiguration>();
            if (config.AllowRegistrations == false)
            {
                ModelState.AddModelError("", "New registrations are not allowed.");
            }

            if (!ModelState.IsValid)
            {
                return new RegisterResult {Success = false, ErrorMessage = ModelState.ToSummary()};
            }


            try
            {
                var reply = await _accountService.ValidateLogin(model.Email, model.UserName);

                if (reply.UserNameIsTaken)
                    ModelState.AddModelError("UserName", "Username is already in use.");

                if (reply.EmailIsTaken)
                    ModelState.AddModelError("Email", "Email address is already in use.");

                if (!ModelState.IsValid)
                    return new RegisterResult { Success = false, ErrorMessage = ModelState.ToSummary() };

                // This is really a workaround, but the UnitOfWork that wraps 
                // this action method deadlocks our transaction in the message bus,
                // thus we need to tell that the outer UoW is done.
                _uow.SaveChanges();

                await
                    _messageBus.SendAsync(User, new RegisterAccount(model.UserName, model.Password, model.Email)
                    {
                        ReturnUrl = model.ReturnUrl
                    });
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("UserName", exception.Message);
                return new RegisterResult { Success = false, ErrorMessage = ModelState.ToSummary() };
            }


            return new RegisterResult {Success = true, VerificationIsRequested = true};
        }

        [HttpGet("password/request/reset")]
        public ActionResult RequestPasswordReset()
        {
            return View();
        }

        [HttpPost("api/password/reset")]
        public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var cmd = new RequestPasswordReset(model.EmailAddress);
            await _messageBus.SendAsync(User, cmd);

            return View("PasswordRequestReceived");
        }

        [HttpGet("api/password/reset/{activationKey}")]
        public ActionResult ResetPassword(string activationKey)
        {
            return View(new ResetPasswordViewModel { ActivationKey = activationKey });
        }

        [HttpPost("api/password/reset/{activationKey}")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.Password != model.Password2)
            {
                ModelState.AddModelError("Password2", "Passwords must match.");
            }
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var found = await _accountService.ResetPassword(model.ActivationKey, model.Password);
                if (!found)
                {
                    ModelState.AddModelError("", "Activation key was not found.");
                    return View(model);
                }
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
                _logger.Error("Failed to reset password using key " + model.ActivationKey, exception);
                return View(model);
            }

            var drDictionary =
                new RouteValueDictionary { { "usernote", "Password have been changed, you may now login." } };
            return RedirectToAction("Login", drDictionary);
        }

        [Authorize, HttpPost("account/update/token")]
        public async Task<LoginResult> UpdateSession()
        {
            var getApps = new GetApplicationList { AccountId = User.GetAccountId() };
            var apps = await _queryBus.QueryAsync(User, getApps);

            var currentClaims = User.Claims
                .Where(x => x.Type != CoderrClaims.Application && x.Type != CoderrClaims.ApplicationAdmin)
                .ToList();

            foreach (var app in apps)
            {
                var claim = new Claim(CoderrClaims.Application, app.Id.ToString());
                currentClaims.Add(claim);

                if (app.IsAdmin)
                {
                    claim = new Claim(CoderrClaims.ApplicationAdmin, app.Id.ToString());
                    currentClaims.Add(claim);
                }
            }

            var identity = new ClaimsIdentity(currentClaims, "Cookies");
            var token = JwtHelper.GenerateToken(identity);

            return new LoginResult { Success = true, JwtToken = token };
        }

    }
}