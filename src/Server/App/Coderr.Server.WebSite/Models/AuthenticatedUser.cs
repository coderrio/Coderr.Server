#nullable enable
using Coderr.Server.Api.Core.Applications;

namespace Coderr.Server.WebSite.Models
{
    public class AuthenticatedUser
    {
        public AuthenticatedUser(int accountId, string? userName)
        {
            AccountId = accountId;
            UserName = userName;
            Applications = new ApplicationListItem[0];
            LicenseText = "";
        }

        public int AccountId { get; private set; }
        public string? UserName { get; private set; }
        public ApplicationListItem[] Applications { get; set; }
        public bool IsSysAdmin { get; set; }
        public string LicenseText { get; set; }
    }
}
