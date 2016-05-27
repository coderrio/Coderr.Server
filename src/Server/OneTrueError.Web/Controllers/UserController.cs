using System.Web.Mvc;

namespace OneTrueError.Web.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult Notifications()
        {
            return View();
        }
    }
}