using System.Security.Claims;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    /// <summary>
    ///     To serialize claims
    /// </summary>
    public class ClaimDto
    {
        public ClaimDto(Claim claim)
        {
            Value = claim.Value;
            ValueType = claim.ValueType;
            ClaimType = claim.Type;
        }

        protected ClaimDto()
        {
        }

        public string ClaimType { get; set; }


        public string Value { get; set; }
        public string ValueType { get; set; }

        public Claim ToClaim()
        {
            return new Claim(ClaimType, Value, ValueType);
        }
    }
}