using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.WebSite.Models.Accounts
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string ActivationKey { get; set; }

        [Required, Compare("Password2")]
        public string Password { get; set; }

        [Display(Name = "Retype password"), Required]
        public string Password2 { get; set; }
    }
}