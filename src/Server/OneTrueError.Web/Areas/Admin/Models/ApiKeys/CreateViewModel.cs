using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OneTrueError.Web.Areas.Admin.Models.ApiKeys
{
    public class CreateViewModel
    {
        public Dictionary<string,string> Applications { get; set; }


        [Required, Display(Name = "Application name")]
        public string ApplicationName { get; set; }

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