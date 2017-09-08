using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Griffin.Data;
using Microsoft.Owin.Security;
using OneTrueError.Api.Core.Applications;
using OneTrueError.App.Core.Accounts;
using OneTrueError.App.Core.Applications;
using OneTrueError.App.Core.Users;
using OneTrueError.Infrastructure;
using OneTrueError.Infrastructure.Security;
using OneTrueError.SqlServer.Core.Accounts;
using OneTrueError.SqlServer.Core.Applications;
using OneTrueError.SqlServer.Core.Users;
using OneTrueError.Web.Areas.Installation.Models;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
    [OutputCache(Duration = 0, NoStore = true)]
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

                var tm = new ApplicationTeamMember(app.Id, account.Id)
                {
                    AddedByName = "System",
                    Roles = new[] {ApplicationRole.Admin, ApplicationRole.Member},
                    UserName = account.UserName
                };
                await repos2.CreateAsync(tm);

                uow.SaveChanges();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(ClaimTypes.Name, account.UserName, ClaimValueTypes.String),
                    new Claim(ClaimTypes.Email, account.Email, ClaimValueTypes.String),
                    new Claim(OneTrueClaims.Application, app.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(OneTrueClaims.ApplicationAdmin, app.Id.ToString(), ClaimValueTypes.Integer32),
                    new Claim(ClaimTypes.Role, OneTrueClaims.RoleSysAdmin, ClaimValueTypes.String)
                };
                var identity = new ClaimsIdentity(claims, "Cookie", ClaimTypes.Name, ClaimTypes.Role);
                var properties = new AuthenticationProperties {IsPersistent = false};
                HttpContext.GetOwinContext().Authentication.SignIn(properties, identity);

                return Redirect(Url.GetNextWizardStep());
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex;
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.PrevLink = Url.GetPreviousWizardStepLink();
            ViewBag.NextLink = Url.GetNextWizardStepLink();
            base.OnActionExecuting(filterContext);
        }

        private void SetStateFlag()
        {
            ViewBag.Exception = null;
            ViewBag.AlreadyCreated = false;
            if (User.Identity.IsAuthenticated)
                ViewBag.AlreadyCreated = true;
            else
            {
                using (var con = SetupTools.DbTools.OpenConnection())
                {
                    using (var uow = new AdoNetUnitOfWork(con))
                    {
                        var id = uow.ExecuteScalar("SELECT TOP 1 Id FROM Accounts");
                        if (id != null)
                            ViewBag.AlreadyCreated = true;
                    }
                }
            }

            if (!ViewBag.AlreadyCreated)
                ViewBag.NextLink = null;
        }
    }
}