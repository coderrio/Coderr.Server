using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coderr.Server.Web.Areas.Installation
{
    public class InstallationOptions
    {
        public bool IsConfigured { get; set; }
        public string Password { get; set; }
    }
}
