using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Coderr.Server.WebSite.Models;
using Microsoft.AspNetCore.Identity;

namespace Coderr.Server.WebSite.Identity
{
    public class UserClaimsFactory : IUserClaimsPrincipalFactory<ApplicationUser>
    {
        public Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var identity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email)
            });
            return Task.FromResult<ClaimsPrincipal>(new ClaimsPrincipal(identity));
        }
    }
}