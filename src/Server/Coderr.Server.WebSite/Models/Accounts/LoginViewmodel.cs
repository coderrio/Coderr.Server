using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.WebSite.Models.Accounts
{
    public class LoginViewModel
    {
        [Required]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Allow new users to register. (view only)
        /// </summary>
        public bool AllowRegistrations { get; set; }
    }
}