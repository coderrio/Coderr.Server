using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web2.Models.Accounts
{
    public class AcceptViewModel : RegisterViewModel
    {
        [Required]
        public string InvitationKey { get; set; }
    }
}