﻿using System.Collections.Generic;
using System.Security.Claims;
using Coderr.Server.Infrastructure.Security;

namespace Coderr.Server.App.Tests
{
    class PrincipalHelper
    {
        public static ClaimsPrincipal Create(int userId, string userName)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString(), ClaimValueTypes.Integer32),
            };
            var identity = new ClaimsIdentity(claims, AuthenticationTypes.Default);
            return new ClaimsPrincipal(identity);
        }
    }
}
