using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Areas.Installation.Models
{
    public class SupportViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        public string CompanyName { get; set; }
    }
}