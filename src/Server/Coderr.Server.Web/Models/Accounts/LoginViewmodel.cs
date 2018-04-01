using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Models.Accounts
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        [Required]
        public string UserName { get; set; }

        public string ReturnUrl { get; set; }

        /// <summary>
        /// Allow new users to register. (view only)
        /// </summary>
        public bool AllowRegistrations { get; set; }
    }
}