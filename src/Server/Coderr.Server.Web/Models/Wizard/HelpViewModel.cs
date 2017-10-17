using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace codeRR.Server.Web.Models.Wizard
{
    public class HelpViewModel
    {
        [Required]
        public string Message { get; set; }

        public string Referer { get; set; }
    }
}