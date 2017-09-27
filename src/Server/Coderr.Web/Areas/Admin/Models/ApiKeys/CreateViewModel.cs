using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Admin.Models.ApiKeys
{
    public class CreateViewModel
    {
        [Required, Display(Name = "Application name")]
        public string ApplicationName { get; set; }

        public Dictionary<string, string> AvailableApplications { get; set; }

        public bool ReadOnly { get; set; }

        [Display(Name = "Selected applications")]
        public string[] SelectedApplications { get; set; }
    }
}

/*function S4() {
    return (((1+Math.random())*0x10000)|0).toString(16).substring(1); 
}
 
// then to call it, plus stitch in '4' in the third group
guid = (S4() + S4() + "-" + S4() + "-4" + S4().substr(0,3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
*/