using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace codeRR.Server.Web.Models.Wizard
{
    public class PackagesViewModel
    {
        public int ApplicationId { get; set; }
        public string ErrorMessage { get; set; }
        public NugetPackage[] Packages { get; set; }
        public string LibraryName { get; set; }
    }
}