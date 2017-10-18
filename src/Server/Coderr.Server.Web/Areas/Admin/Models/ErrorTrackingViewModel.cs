using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Admin.Models
{
    public class ErrorTrackingViewModel
    {
        [Display(Name = "Contact email"), EmailAddress]
        public string ContactEmail { get; set; }
    }
}