using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.WebSite.Models.Accounts
{
    public class RequestPasswordResetViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}