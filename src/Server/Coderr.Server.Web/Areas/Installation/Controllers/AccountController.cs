using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.Applications;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Infrastructure;
using Coderr.Server.SqlServer.Core.Accounts;
using Coderr.Server.SqlServer.Core.Applications;
using Coderr.Server.SqlServer.Core.Users;
using Coderr.Server.Web.Areas.Installation.Models;
using Griffin.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coderr.Server.Web.Areas.Installation.Controllers
{
    [Area("Installation")]

    public class AccountController : Controller
    {
        public ActionResult Admin()
        {
            SetStateFlag();
            var model = new AccountViewModel();
            return View(model);
        }


        [HttpPost]
        public async Task<ActionResult> Admin(AccountViewModel model)
        {
            SetStateFlag();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var account = new Account(model.UserName, model.Password);
                account.Activate();
                account.IsSysAdmin = true;
                var con = SetupTools.DbTools.OpenConnection();
                var uow = new AdoNetUnitOfWork(con);
                var repos = new AccountRepository(uow);
                if (await repos.IsUserNameTakenAsync(model.UserName))
                    return Redirect(Url.GetNextWizardStep());

                account.SetVerifiedEmail(model.EmailAddress);
                await repos.CreateAsync(account);

                var user = new User(account.Id, account.UserName)
                {
                    EmailAddress = account.Email
                };
                var userRepos = new UserRepository(uow);
                await userRepos.CreateAsync(user);

                var repos2 = new ApplicationRepository(uow);
                var app = new Application(user.AccountId, "DemoApp")
                {
                    ApplicationType = TypeOfApplication.DesktopApplication
                };
                await repos2.CreateAsync(app);

                var tm = new ApplicationTeamMember(app.Id, account.Id, "System")
                {
                    Roles = new[] { ApplicationRole.Admin, ApplicationRole.Member },
                    UserName = account.UserName
                };
                await repos2.CreateAsync(tm);

                uow.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(ClaimTypes.Name, account.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Email, account.Email, ClaimValueTypes.String),
                    new Claim(CoderrClaims.Application, app.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(CoderrClaims.ApplicationAdmin, app.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(ClaimTypes.Role, CoderrRoles.SysAdmin, ClaimValueTypes.String)
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties { IsPersistent = false };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity), properties);

                return Redirect(Url.GetNextWizardStep());
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        private void SetStateFlag()
        {
            ViewBag.Exception = null;
            ViewBag.AlreadyCreated = false;
            using (var con = SetupTools.DbTools.OpenConnection())
            {
                using (var uow = new AdoNetUnitOfWork(con))
                {
                    var id = uow.ExecuteScalar("SELECT TOP 1 Id FROM Accounts");
                    if (id != null)
                        ViewBag.AlreadyCreated = true;
                }
            }

            if (!ViewBag.AlreadyCreated)
                ViewBag.NextLink = null;
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            Response.Headers.Add("Cache-Control", "no-cache, no-store");
            Response.Headers.Add("Expires", "-1");
            return base.OnActionExecutionAsync(context, next);
        }
    }
}