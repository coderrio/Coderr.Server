using System.ComponentModel.DataAnnotations;

namespace OneTrueError.Web.Models.Account
{
    public class ResetPasswordViewModel
    {
        [Required, Compare("Password2")]
        public string Password { get; set; }

        [Display(Name = "Retype password")]
        public string Password2 { get; set; }

        [Required]
        public string ActivationKey { get; set; }
    }
}