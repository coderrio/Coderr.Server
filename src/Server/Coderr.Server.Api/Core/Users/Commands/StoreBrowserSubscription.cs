namespace Coderr.Server.Api.Core.Users.Commands
{
    /// <summary>
    /// https://tools.ietf.org/html/draft-ietf-webpush-encryption-08
    /// </summary>
    [Message]
    public class StoreBrowserSubscription
    {
        public int UserId { get; set; }
        public string Endpoint { get; set; }

        public string PublicKey { get; set; }
        public string AuthenticationSecret { get; set; }

        public int? ExpirationTime { get; set; }
    }
}
