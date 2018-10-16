using Coderr.Server.Api.Core.Applications;

namespace Coderr.Server.Web.Models.Users
{
    public class AuthenticatedUser
    {
        public int AccountId { get; set; }
        public string UserName { get; set; }
        public ApplicationListItem[] Applications { get; set; }
    }
}