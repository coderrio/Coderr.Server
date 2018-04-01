namespace Coderr.Server.Web.Infrastrucutre.Results
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