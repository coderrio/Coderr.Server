using System;
using System.Web.Mvc;

namespace codeRR.Server.Web.Controllers
{
    /// <summary>
    ///     Used to be able to load views by using ajax requests.
    /// </summary>
    public class PageController : Controller
    {
        public ActionResult Index(string path)
        {
            if (!path.StartsWith("/views", StringComparison.OrdinalIgnoreCase))
                return new HttpStatusCodeResult(403);

            return PartialView("~" + path);
        }
    }
}