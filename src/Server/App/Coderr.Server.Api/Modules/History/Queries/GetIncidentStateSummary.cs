namespace Coderr.Server.Api.Modules.History.Queries
{
    [Message]
    public class GetIncidentStateSummary
    {
        public int ApplicationId { get; set; }
        public string ApplicationVersion { get; set; }
    }
}