namespace Coderr.Server.Web.Infrastructure.Results
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