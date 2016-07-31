using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Griffin.Data;
using OneTrueError.App.Core.Accounts;
using OneTrueError.App.Core.Users;
using OneTrueError.Infrastructure;
using OneTrueError.SqlServer.Core.Accounts;
using OneTrueError.SqlServer.Core.Users;
using OneTrueError.Web.Areas.Installation.Models;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Areas.Installation.Controllers
{
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

                uow.SaveChanges();
                SessionUser.Current = new SessionUser(account.Id, model.UserName);
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
            if (SessionUser.Current != null)
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