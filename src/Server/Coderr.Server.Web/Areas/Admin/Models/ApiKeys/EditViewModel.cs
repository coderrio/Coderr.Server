using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Admin.Models.ApiKeys
{
    public class EditViewModel
    {
        public int Id { get; set; }

        [Required, Display(Name = "Application name")]
        public string ApplicationName { get; set; }

        public Dictionary<string, string> AvailableApplications { get; set; }

        public bool ReadOnly { get; set; }

        [Display(Name = "Selected applications")]
        public string[] SelectedApplications { get; set; }
    }
}
