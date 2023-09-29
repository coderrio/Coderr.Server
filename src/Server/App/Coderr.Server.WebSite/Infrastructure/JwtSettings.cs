using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Coderr.Server.WebSite.Infrastructure
{
    public class JwtSettings
    {
        static JwtSettings()
        {
            var mySecret = "fs9fsd@£20@lffjLDdjsaSLS%&¤#";
            TokenKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(mySecret));

            Issuer = "https://coderr.io";
            Audience = "https://coderr.io";
        }

        public static string Audience { get; }

        public static string Issuer { get; }

        public static SymmetricSecurityKey TokenKey { get; }
    }
}