using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Microsoft.AspNetCore.Mvc;

namespace Coderr.Server.WebSite.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            if (!HostConfig.Instance.IsConfigured)
            {
                return Redirect("~/Installation");
            }

            return View();
        }
        [HttpGet("wazzaa")]
        public IActionResult Wazzaa()
        {
            if (!HostConfig.Instance.IsConfigured)
            {
                return NoContent();
            }

            return Ok(_configuration["LoginUrl"] ?? "/account/login");
        }
    }
}
