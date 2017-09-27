using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string ActivationKey { get; set; }

        [Required, Compare("Password2")]
        public string Password { get; set; }

        [Display(Name = "Retype password")]
        public string Password2 { get; set; }
    }
}