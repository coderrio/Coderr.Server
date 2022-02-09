using System.Threading.Tasks;
using Coderr.Server.WebSite.Models;
using Microsoft.AspNetCore.Identity;

namespace Coderr.Server.WebSite.Identity
{
    public class UserConfirmation : IUserConfirmation<ApplicationUser>
    {
        public Task<bool> IsConfirmedAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }
    }
}