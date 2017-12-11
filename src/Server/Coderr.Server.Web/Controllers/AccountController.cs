using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using codeRR.Server.Api.Core.Accounts.Commands;
using codeRR.Server.Api.Core.Accounts.Requests;
using codeRR.Server.Api.Core.Applications;
using codeRR.Server.Api.Core.Applications.Queries;
using codeRR.Server.Api.Core.Invitations.Queries;
using codeRR.Server.App.Configuration;
using codeRR.Server.App.Core.Accounts;
using codeRR.Server.Infrastructure.Configuration;
using codeRR.Server.Infrastructure.Security;
using codeRR.Server.Web.Models.Account;
using Coderr.Server.PluginApi.Config;
using DotNetCqs;
using Griffin.Data;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace codeRR.Server.Web.Controllers
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
        private IMessageBus _messageBus;
        private IQueryBus _queryBus;
        private ConfigurationStore _configStore;

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
        [Route("account/accept/{id}")]
        [HttpGet]
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
                ModelState.AddModelError("", exception);
                _logger.Error("Failed ot launch", exception);
                return View(new AcceptViewModel());
            }
        }

        [HttpPost]
        [Route("account/accept")]
        public async Task<ActionResult> Accept(AcceptViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cmd = new AcceptInvitation(model.UserName, model.Password, model.InvitationKey)
            {
                AcceptedEmail = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var identity = await _accountService.AcceptInvitation(this.ClaimsUser(), cmd);
            if (identity == null)
            {
                ModelState.AddModelError("",
                    "Failed to find an invitation with the specified key. You might have already accepted the invitation? If not, ask for a new one.");
                _logger.Error("Failed to find invitation " + model.InvitationKey);
                return View(new AcceptViewModel());
            }

          
            SignIn(identity);
            return Redirect("~/#/account/accepted");
        }

        public async Task<ActionResult> Activate(string id)
        {
            try
            {
                var identity = await _accountService.ActivateAccount(this.ClaimsUser(), id);
                SignIn(identity);
                return Redirect(new ClaimsPrincipal(identity).IsSysAdmin()
                    ? "~/#/welcome/admin/"
                    : "~/#/welcome/user/");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                return View();
            }
        }

        public ActionResult ActivationRequested()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            var config = _configStore.Load<BaseConfiguration>();
            var model = new LoginViewModel
            {
                AllowRegistrations = config.AllowRegistrations != false
            };

            var url = ConfigurationManager.AppSettings["LoginUrl"];
            if (url != null && url != Request.Url.AbsolutePath)
                return Redirect(url);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            var config = _configStore.Load<BaseConfiguration>();
            model.AllowRegistrations = config.AllowRegistrations != false;

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var principal = await _accountService.Login(model.UserName, model.Password);
                if (principal == null)
                {
                    ModelState.AddModelError("", "Incorrect username or password.");
                    model.Password = "";
                    return View(model);
                }

             
                SignIn(principal);

                if (model.ReturnUrl != null && model.ReturnUrl.StartsWith("/"))
                    return Redirect(model.ReturnUrl);
                return Redirect("~/#/");
            }
            catch (AuthenticationException err)
            {
                _logger.Error("Failed to authenticate", err);
                ModelState.AddModelError("", err.Message);
                return View();
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to authenticate", exception);
                ModelState.AddModelError("", "Failed to authenticate");
                return View();
            }
        }

        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session.Abandon();

            return Redirect("~/");
        }

        public ActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var config = _configStore.Load<BaseConfiguration>();
            if (config.AllowRegistrations == false)
            {
                ModelState.AddModelError("", "New registrations are not allowed.");
            }

            if (!ModelState.IsValid)
                return View(model);


            try
            {
                var reply = await _accountService.ValidateLogin(model.Email, model.UserName);

                if (reply.UserNameIsTaken)
                    ModelState.AddModelError("UserName", "Username is already in use.");

                if (reply.EmailIsTaken)
                    ModelState.AddModelError("Email", "Email address is already in use.");

                if (!ModelState.IsValid)
                    return View(model);

                // This is really a workaround, but the UnitOfWork that wraps 
                // this action method deadlocks our transaction in the message bus,
                // thus we need to tell that the outer UoW is done.
                _uow.SaveChanges();

                await
                    _messageBus.SendAsync(this.ClaimsUser(), new RegisterAccount(model.UserName, model.Password, model.Email));
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("UserName", exception.Message);
                return View("Register");
            }


            return RedirectToAction("ActivationRequested");
        }

        [Route("password/request/reset")]
        public ActionResult RequestPasswordReset()
        {
            return View();
        }

        [Route("password/request/reset")]
        [HttpPost]
        public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var cmd = new RequestPasswordReset(model.EmailAddress);
            await _messageBus.SendAsync(this.ClaimsUser(), cmd);

            return View("PasswordRequestReceived");
        }

        [Route("password/reset/{activationKey}")]
        public ActionResult ResetPassword(string activationKey)
        {
            return View(new ResetPasswordViewModel { ActivationKey = activationKey });
        }

        [Route("password/reset")]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
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


        [Authorize]
        public async Task<ActionResult> UpdateSession(string returnUrl = null)
        {
            var getApps = new GetApplicationList { AccountId = User.GetAccountId() };
            var apps = await _queryBus.QueryAsync(getApps);
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var identity = CreateIdentity(User.GetAccountId(), User.Identity.Name, User.IsSysAdmin(), apps);
            SignIn(identity);

            if (returnUrl != null)
                return Redirect(returnUrl);

            return new EmptyResult();
        }

        private static ClaimsIdentity CreateIdentity(int accountId, string userName, bool isSysAdmin, ApplicationListItem[] apps)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String)
            };
            foreach (var app in apps)
            {
                claims.Add(new Claim(CoderrClaims.Application, app.Id.ToString(), ClaimValueTypes.Integer32));
                claims.Add(new Claim(CoderrClaims.ApplicationName, app.Name, ClaimValueTypes.String));
                if (app.IsAdmin)
                    claims.Add(new Claim(CoderrClaims.ApplicationAdmin, app.Id.ToString(), ClaimValueTypes.Integer32));
            }

            //accountId == 1 for backwards compatibility (with version 1.0)
            var roles = isSysAdmin || accountId == 1 ? new[] { CoderrClaims.RoleSysAdmin } : new string[0];
            var context = new PrincipalFactoryContext(accountId, userName, roles)
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Claims = claims.ToArray()
            };
            return (ClaimsIdentity)PrincipalFactory.CreateAsync(context).Result.Identity;
        }

        private void SignIn(ClaimsIdentity identity)
        {
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
            };
            var ctx = Request.GetOwinContext();
            var mvcIdentity = new ClaimsIdentity(identity.Claims, DefaultAuthenticationTypes.ApplicationCookie);
            ctx.Authentication.SignIn(props, mvcIdentity);
        }
    }
}