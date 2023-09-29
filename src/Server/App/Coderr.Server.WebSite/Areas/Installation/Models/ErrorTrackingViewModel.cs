using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.WebSite.Areas.Installation.Models
{
    public class ErrorTrackingViewModel
    {
        [Display(Name = "Contact email"), EmailAddress]
        public string ContactEmail { get; set; }

    }
}