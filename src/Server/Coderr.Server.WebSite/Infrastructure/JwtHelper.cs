using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class JwtHelper
    {
        public static void Configure(JwtBearerOptions x)
        {
            x.IncludeErrorDetails = true;
            x.RequireHttpsMetadata = false;
            x.Audience = JwtSettings.Audience;
            x.ClaimsIssuer = JwtSettings.Issuer;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = JwtSettings.TokenKey,
                ValidAudience = JwtSettings.Audience,
                ValidIssuer = JwtSettings.Issuer
            };
        }

        public static string GenerateToken(ClaimsIdentity identity)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = JwtSettings.Issuer,
                Audience = JwtSettings.Audience,

                SigningCredentials = new SigningCredentials(JwtSettings.TokenKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
