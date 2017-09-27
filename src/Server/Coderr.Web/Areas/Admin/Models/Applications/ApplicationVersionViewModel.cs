using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace codeRR.Web.Areas.Admin.Models.Applications
{
    public class ApplicationVersionViewModel
    {
        public SelectListItem[] Assemblies { get; set; }

        [Required]
        public int ApplicationId { get; set; }

        [Required]
        public string SelectedAssembly { get; set; }
    }
}