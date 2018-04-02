using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Areas.Installation.Models
{
    public class ErrorTrackingViewModel
    {
        [Display(Name = "Contact email"), EmailAddress]
        public string ContactEmail { get; set; }

    }
}