using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using DotNetCqs;
using log4net;
using OneTrueError.Api.Core.Accounts;
using OneTrueError.Api.Core.Accounts.Commands;
using OneTrueError.Api.Core.Accounts.Requests;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Api.Core.Invitations.Queries;
using OneTrueError.App;
using OneTrueError.App.Configuration;
using OneTrueError.Infrastructure.Configuration;
using OneTrueError.Web.Models;
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

        [Route("password/request/reset")]
        public ActionResult RequestPasswordReset()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Simple(string email, string emailAddress)
        {
            var cmd = new RegisterSimple(email ?? emailAddress);
            await _commandBus.ExecuteAsync(cmd);
            return View("Simple");
        }

        [Route("password/request/reset"), HttpPost]
        public async Task<ActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var cmd = new RequestPasswordReset(model.EmailAddress);
            await _commandBus.ExecuteAsync(cmd);

            return View("PasswordRequestReceived");
        }


        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [Route("password/reset/{activationKey}")]
        public ActionResult ResetPassword(string activationKey)
        {
            return View(new ResetPasswordViewModel { ActivationKey = activationKey });
        }

        [Route("password/reset"), HttpPost]
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

            var drDictionary = new RouteValueDictionary();
            drDictionary.Add("toastr.info", "Password have been changed, you may now login.");
            return RedirectToAction("Login", drDictionary);
        }

        /// <summary>
        ///     Accept invitation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("account/accept/{id}"), HttpGet]
        public async Task<ActionResult> Accept(string id)
        {
            try
            {
                var query = new GetInvitationByKey(id);
                var invitation = await _queryBus.QueryAsync(query);
                if (invitation == null)
                {
                    return View("InvitationNotFound");
                }

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

        [HttpPost, Route("account/accept")]
        public async Task<ActionResult> Accept(AcceptViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);


            var cmd = new AcceptInvitation(model.UserName, model.Password, model.InvitationKey)
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var reply = await _requestReplyBus.ExecuteAsync(cmd);
            if (reply == null)
            {
                ModelState.AddModelError("", "Failed to find an invitation with the specified key. You might have already accepted the invitation? If not, ask for a new one.");
                _logger.Error("Failed to find invitation " + model.InvitationKey);
                return View(new AcceptViewModel());
            }

            FormsAuthentication.SetAuthCookie(reply.UserName, false);
            var user = new SessionUser(reply.AccountId, reply.UserName);
            SessionUser.Current = user;

            var getApps = new GetApplicationList();
            var apps = await _queryBus.QueryAsync(getApps);
            SessionUser.Current.Applications = apps.ToDictionary(x => x.Id, x => x.Name);
            SessionUser.Current.Applications[0] = "Dashboard";

            return Redirect("~/#/account/accepted");
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();

            return Redirect("~/");
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
                            "Your account is locked or have not been activated (check your email account). Contact support@onetrueerror.com if you need assistance.");

                    model.Password = "";
                    return View(model);
                }

                FormsAuthentication.SetAuthCookie(reply.UserName, model.RememberMe);
                var user = new SessionUser(reply.AccountId, reply.UserName);
                SessionUser.Current = user;

                var getApps = new GetApplicationList() { AccountId = user.AccountId };
                var apps = await _queryBus.QueryAsync(getApps);
                SessionUser.Current.Applications = apps.ToDictionary(x => x.Id, x => x.Name);
                SessionUser.Current.Applications[0] = "Dashboard";

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

        [HttpGet]
        public async Task<ActionResult> CookieLogin(string returnTo)
        {
            if (string.IsNullOrEmpty(returnTo) && !string.IsNullOrEmpty(Request.QueryString["ReturnTo"]))
            {
                Debugger.Break();
                returnTo = Request.QueryString["ReturnTo"];
            }

            try
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie == null)
                    return RedirectToAction("Login", new { ReturnTo = returnTo });


                var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                if (ticket == null || (ticket.Expired && ticket.IsPersistent))
                {
                    return RedirectToAction("Login", new { ReturnTo = returnTo });
                }


                var login = new Login(ticket.Name, null);
                var reply = await _requestReplyBus.ExecuteAsync(login);
                if (reply == null)
                {
                    _logger.Error("Result is NULL :(");
                    ModelState.AddModelError("",
                        "Internal error.");

                    return RedirectToAction("Login", new { ReturnTo = returnTo });
                }

                if (reply.Result != LoginResult.Successful)
                {
                    var config = ConfigurationStore.Instance.Load<BaseConfiguration>();

                    var errorMessage = reply.Result == LoginResult.IncorrectLogin
                        ? "Incorrect username or password."
                        : string.Format(
                            "Your account have not been activated (check your email account). Contact {0} if you need assistance.",
                            config.SupportEmail);

                    ModelState.AddModelError("", errorMessage);
                    return RedirectToAction("Login", new { ReturnTo = returnTo });
                }

                var user = new SessionUser(reply.AccountId, reply.UserName);
                SessionUser.Current = user;

                var getApps = new GetApplicationList();
                var apps = await _queryBus.QueryAsync(getApps);
                SessionUser.Current.Applications = apps.ToDictionary(x => x.Id, x => x.Name);
                SessionUser.Current.Applications[0] = "Dashboard";
                if (string.IsNullOrEmpty(returnTo))
                    return Redirect("~/#/");
                return Redirect(returnTo);
            }
            catch (AuthenticationException err)
            {
                _logger.Error("Failed to authenticate", err);
                ModelState.AddModelError("", err.Message);
                return View("Login");
            }
            catch (Exception exception)
            {
                _logger.Error("Failed to authenticate", exception);
                ModelState.AddModelError("", "Failed to authenticate");
                return View("Login");
            }
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

        public ActionResult ActivationRequested()
        {
            return View();
        }

        public async Task<ActionResult> Activate(string id)
        {
            try
            {
                var reply = await _requestReplyBus.ExecuteAsync(new ActivateAccount(id));
                FormsAuthentication.SetAuthCookie(reply.UserName, false);
                var user = new SessionUser(reply.AccountId, reply.UserName);
                SessionUser.Current = user;
                var getApps = new GetApplicationList();
                var apps = await _queryBus.QueryAsync(getApps);
                SessionUser.Current.Applications = apps.ToDictionary(x => x.Id, x => x.Name);
                SessionUser.Current.Applications[0] = "Dashboard";


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
    }
}