using System.ComponentModel.DataAnnotations;

namespace OneTrueError.Web.Models.Account
{
    public class AcceptViewModel : RegisterViewModel
    {
        [Required]
        public string InvitationKey { get; set; }
    }
}