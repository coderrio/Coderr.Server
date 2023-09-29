using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.WebSite.Models.Accounts
{
    public class AcceptViewModel : RegisterViewModel
    {
        [Required]
        public string InvitationKey { get; set; }
    }
}