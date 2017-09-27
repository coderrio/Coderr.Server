using System.ComponentModel.DataAnnotations;

namespace codeRR.Server.Web.Models.Account
{
    public class AcceptViewModel : RegisterViewModel
    {
        [Required]
        public string InvitationKey { get; set; }
    }
}