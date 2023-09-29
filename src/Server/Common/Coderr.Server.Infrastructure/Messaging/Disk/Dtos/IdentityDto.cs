using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Dtos
{
    /// <summary>
    ///     Claim serialization
    /// </summary>
    public class IdentityDto
    {
        public IdentityDto(ClaimsIdentity identity)
        {
            AuthenticationType = identity.AuthenticationType;
            NameClaimType = identity.NameClaimType;
            RoleClaimType = identity.RoleClaimType;
            Claims = identity.Claims.Select(x => new ClaimDto(x)).ToList();
        }

        protected IdentityDto()
        {
        }

        public string AuthenticationType { get; set; }
        public IReadOnlyList<ClaimDto> Claims { get; set; }
        public string NameClaimType { get; set; }
        public string RoleClaimType { get; set; }

        public ClaimsIdentity ToIdentity()
        {
            var claims = Claims.Select(x => new Claim(x.ClaimType, x.Value, x.ValueType));
            return new ClaimsIdentity(claims, AuthenticationType, NameClaimType, RoleClaimType);
        }
    }
}