using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetCqs;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using OneTrueError.Api.Core.Accounts;
using OneTrueError.Api.Core.Accounts.Commands;
using OneTrueError.Api.Core.Accounts.Requests;
using OneTrueError.Api.Core.Applications;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Api.Core.Invitations.Queries;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Infrastructure.Security;
using OneTrueError.Web.Models.Account;

namespace OneTrueError.Web.Controllers
{
    /// <summary>
    ///     TODO: Break out logic
    /// </summary>
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly ICommandBus _commandBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof(AccountController));
        private readonly IQueryBus _queryBus;
        private readonly IRequestReplyBus _requestReplyBus;

        public AccountController(ICommandBus commandBus, IQueryBus queryBus, IRequestReplyBus requestReplyBus)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _requestReplyBus = requestReplyBus;
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

            var query = new GetInvitationByKey(model.InvitationKey);
            var invitation = await _queryBus.QueryAsync(query);
            if (invitation == null)
                return View("InvitationNotFound");

            var cmd = new AcceptInvitation(model.UserName, model.Password, model.InvitationKey)
            {
                AcceptedEmail = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var reply = await _requestReplyBus.ExecuteAsync(cmd);
            if (reply == null)
            {
                ModelState.AddModelError("",
                    "Failed to find an invitation with the specified key. You might have already accepted the invitation? If not, ask for a new one.");
                _logger.Error("Failed to find invitation " + model.InvitationKey);
                return View(new AcceptViewModel());
            }

            var getApps = new GetApplicationList {AccountId = reply.AccountId};
            var apps = await _queryBus.QueryAsync(getApps);


            var identity = CreateIdentity(reply.AccountId, reply.UserName, false, apps);
            SignIn(identity);
            return Redirect("~/#/account/accepted");
        }

        public async Task<ActionResult> Activate(string id)
        {
            try
            {
                var reply = await _requestReplyBus.ExecuteAsync(new ActivateAccount(id));
                var getApps = new GetApplicationList {AccountId = reply.AccountId};
                var apps = await _queryBus.QueryAsync(getApps);

                var identity = CreateIdentity(reply.AccountId, reply.UserName, false, apps);
                SignIn(identity);


                return Redirect("~/#/welcome");
            }
            catch (Exception err)
            {
                ModelState.AddModelError("", err.Message);
                return View();
            }
        }

        public ActionResult Activated()
        {
            var config = ConfigurationStore.Instance.Load<BaseConfiguration>();
            ViewBag.DashboardUrl = config.BaseUrl + "/welcome";
            return View();
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
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);


            try
            {
                var login = new Login(model.UserName, model.Password);
                var reply = await _requestReplyBus.ExecuteAsync(login);
                if (reply == null)
                {
                    _logger.Error("Result is NULL :(");
                    ModelState.AddModelError("",
                        "Internal error.");

                    model.Password = "";
                    return View(model);
                }
                if (reply.Result != LoginResult.Successful)
                {
                    if (reply.Result == LoginResult.IncorrectLogin)
                        ModelState.AddModelError("", "Incorrect username or password.");
                    else
                        ModelState.AddModelError("",
                            "Your account is locked or have not been activated (check your mailbox). Contact support@onetrueerror.com if you need assistance.");

                    model.Password = "";
                    return View(model);
                }

                var getApps = new GetApplicationList {AccountId = reply.AccountId};
                var apps = await _queryBus.QueryAsync(getApps);
                var identity = CreateIdentity(reply.AccountId, reply.UserName, reply.IsSysAdmin, apps);
                SignIn(identity);

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
            if (!ModelState.IsValid)
                return View(model);


            var request = new ValidateNewLogin
            {
                Email = model.Email,
                UserName = model.UserName
            };
            try
            {
                var reply = await _requestReplyBus.ExecuteAsync(request);

                if (reply.UserNameIsTaken)
                    ModelState.AddModelError("UserName", "Username is already in use.");

                if (reply.EmailIsTaken)
                    ModelState.AddModelError("Email", "Email address is already in use.");

                if (!ModelState.IsValid)
                    return View(model);

                await
                    _commandBus.ExecuteAsync(new RegisterAccount(model.UserName, model.Password, model.Email));
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
            await _commandBus.ExecuteAsync(cmd);

            return View("PasswordRequestReceived");
        }

        [Route("password/reset/{activationKey}")]
        public ActionResult ResetPassword(string activationKey)
        {
            return View(new ResetPasswordViewModel {ActivationKey = activationKey});
        }

        [Route("password/reset")]
        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var request = new ResetPassword(model.ActivationKey, model.Password);
                var reply = await _requestReplyBus.ExecuteAsync(request);
            }
            catch (Exception exception)
            {
                ModelState.AddModelError("", exception.Message);
                _logger.Error("Failed to reset password using key " + model.ActivationKey, exception);
                return View(model);
            }

            var drDictionary =
                new RouteValueDictionary {{"usernote", "Password have been changed, you may now login."}};
            return RedirectToAction("Login", drDictionary);
        }

        [HttpPost]
        public async Task<ActionResult> Simple(string email, string emailAddress)
        {
            var cmd = new RegisterSimple(email ?? emailAddress);
            await _commandBus.ExecuteAsync(cmd);
            return View("Simple");
        }

        public async Task<ActionResult> UpdateSession(string returnUrl = null)
        {
            var getApps = new GetApplicationList {AccountId = User.GetAccountId()};
            var apps = await _queryBus.QueryAsync(getApps);
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            var identity = CreateIdentity(User.GetAccountId(), User.Identity.Name, User.IsSysAdmin(), apps);
            SignIn(identity);

            if (returnUrl != null)
                return Redirect(returnUrl);

            return new EmptyResult();
        }

        private static ClaimsIdentity CreateIdentity(int accountId, string userName,bool isSysAdmin, ApplicationListItem[] apps)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, accountId.ToString(), ClaimValueTypes.Integer32),
                new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String)
            };
            foreach (var app in apps)
            {
                claims.Add(new Claim(OneTrueClaims.Application, app.Id.ToString(), ClaimValueTypes.Integer32));
                claims.Add(new Claim(OneTrueClaims.ApplicationName, app.Name, ClaimValueTypes.String));
                if (app.IsAdmin)
                    claims.Add(new Claim(OneTrueClaims.ApplicationAdmin, app.Id.ToString(), ClaimValueTypes.Integer32));
            }

            //accountId == 1 for backwards compatibility (with version 1.0)
            var roles = isSysAdmin || accountId == 1 ? new[] {OneTrueClaims.RoleSysAdmin} : new string[0];
            var context = new PrincipalFactoryContext(accountId, userName, roles)
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                Claims = claims.ToArray()
            };
            return (ClaimsIdentity) PrincipalFactory.CreateAsync(context).Result.Identity;
        }

        private void SignIn(ClaimsIdentity identity)
        {
            var props = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(14)
            };
            var ctx = Request.GetOwinContext();
            ctx.Authentication.SignIn(props, identity);
        }
    }
}