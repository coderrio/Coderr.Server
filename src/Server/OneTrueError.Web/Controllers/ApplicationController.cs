using System.Threading.Tasks;
using System.Web.Mvc;
using DotNetCqs;
using OneTrueError.Web.Models;

namespace OneTrueError.Web.Controllers
{
    public class ApplicationController : Controller
    {
        private ICommandBus _commandBus;

        public ApplicationController(ICommandBus commandBus)
        {
            _commandBus = commandBus;
        }

        [HttpGet, Route("application/switch/{id}")]
        public ActionResult Switch(int id)
        {
            //await _commandBus.ExecuteAsync(new SwitchApplication(SessionUser.Current.CustomerId, id));
            SessionUser.Current.ApplicationId = id;
            return Redirect("~/application/" + id);
        }

        [HttpGet, Route("application/{id}")]
        public ActionResult Details(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}