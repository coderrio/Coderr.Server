using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Models.Accounts
{
    public class RequestPasswordResetViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}