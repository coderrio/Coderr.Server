using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.Web.Areas.Admin.Controllers
{
    public class TeamController : Controller
    {
        // GET: Admin/Team
        public ActionResult Index()
        {
            return View();
        }
    }
}