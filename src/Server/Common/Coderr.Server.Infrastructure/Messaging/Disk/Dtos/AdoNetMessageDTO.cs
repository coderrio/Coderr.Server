using System;
using System.Collections.Generic;
using System.Security.Claims;
using DotNetCqs;

namespace Coderr.Server.Infrastructure.Messaging.Disk.Dtos
{
    public class MessageDto
    {
        public MessageDto(ClaimsPrincipal principal, Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Properties = message.Properties ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (message.MessageId != Guid.Empty) Properties["X-MessageId"] = message.MessageId.ToString();

            if (message.CorrelationId != Guid.Empty)
                Properties["X-CorrelationId"] = message.CorrelationId.ToString();

            Body = message.Body;
            Properties["X-ContentType"] = message.Body.GetType().FullName;


            ClaimIdentity = principal == null ? null : new IdentityDto((ClaimsIdentity)principal.Identity);
        }

        protected MessageDto()
        {
        }

        public object Body { get; set; }

        public IdentityDto ClaimIdentity { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public void ToMessage(out Message message, out ClaimsPrincipal principal)
        {
            var props = new Dictionary<string, string>(Properties, StringComparer.OrdinalIgnoreCase);
            var identity = ClaimIdentity?.ToIdentity();

            principal = identity != null ? new ClaimsPrincipal(identity) : null;
            message = new Message(Body, props) {MessageId = Guid.Parse(props["X-MessageId"])};

            if (props.TryGetValue("X-CorrelationId", out var correlationId))
            {
                message.CorrelationId = Guid.Parse(correlationId);
                props.Remove("X-CorrelationId");
            }

            props.Remove("X-ContentType");
            props.Remove("X-MessageId");
        }
    }
}