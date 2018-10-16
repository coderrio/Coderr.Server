using System.ComponentModel.DataAnnotations;

namespace Coderr.Server.Web.Models.Accounts
{
    public class AcceptViewModel : RegisterViewModel
    {
        [Required]
        public string InvitationKey { get; set; }
    }
}