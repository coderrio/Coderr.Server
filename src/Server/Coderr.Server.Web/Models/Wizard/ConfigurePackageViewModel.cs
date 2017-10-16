using System;

namespace codeRR.Server.Web.Models.Wizard
{
    public class ConfigurePackageViewModel
    {
        public string AppKey { get; set; }
        public string LibraryName { get; set; }
        public Uri ReportUrl { get; set; }

        public string SharedSecret { get; set; }
        public int ApplicationId { get; set; }
        public string Instruction { get; set; }
        public string ErrorMessage { get; set; }
    }
}