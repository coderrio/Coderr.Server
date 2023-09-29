using Microsoft.AspNetCore.Identity;

namespace Coderr.Server.WebSite.Identity
{
    public class LookupNormalizer : ILookupNormalizer
    {
        public string NormalizeName(string name)
        {
            return name.ToUpperInvariant();
        }

        public string NormalizeEmail(string email)
        {
            return email.ToUpperInvariant();
        }
    }
}