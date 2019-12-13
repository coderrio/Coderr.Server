namespace Coderr.Server.Api.Core.Users.Commands
{
    [Message]
    public class DeleteBrowserSubscription
    {
        public int UserId { get; set; }
        public string Endpoint { get; set; }
    }
}