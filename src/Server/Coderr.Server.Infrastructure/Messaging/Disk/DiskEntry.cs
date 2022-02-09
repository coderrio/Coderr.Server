using DotNetCqs;

namespace Coderr.Server.Infrastructure.Messaging.Disk
{
    public class DiskEntry
    {
        public string AuthenticationType { get; set; }
        public Message Body { get; set; }

        public ClaimDto[] Claims { get; set; }
        public string TypeName { get; set; }
    }
}