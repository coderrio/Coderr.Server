using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Installation.Models
{
    public class BasicsViewModel
    {
        [Required, MinLength(4)]
        public string BaseUrl { get; set; }

        [Required, EmailAddress]
        public string SupportEmail { get; set; }
    }
}