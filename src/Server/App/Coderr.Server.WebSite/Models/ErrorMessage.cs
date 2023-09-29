namespace Coderr.Server.WebSite.Models
{
    public class ErrorMessage
    {
        public ErrorMessage(string reasonPhrase)
        {
            ReasonPhrase = reasonPhrase;
        }

        public string ReasonPhrase { get; }
    }
}