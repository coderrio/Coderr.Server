using System.Security.Claims;
using DotNetCqs;

namespace Coderr.Server.Infrastructure.Messaging.Tests
{
    public class TestMessageWrapper
    {
        public TestMessageWrapper(ClaimsPrincipal principal, Message message)
        {
            Principal = principal;
            Message = message;
        }

        public Message Message { get; }
        public ClaimsPrincipal Principal { get; }
    }
}