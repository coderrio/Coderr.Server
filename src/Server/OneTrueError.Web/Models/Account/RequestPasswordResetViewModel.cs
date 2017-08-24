using System.ComponentModel.DataAnnotations;

namespace OneTrueError.Web.Models.Account
{
    public class RequestPasswordResetViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}