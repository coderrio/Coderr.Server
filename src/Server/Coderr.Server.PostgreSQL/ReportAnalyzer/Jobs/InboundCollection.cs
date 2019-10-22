namespace Coderr.Server.PostgreSQL.ReportAnalyzer.Jobs
{
    public class InboundCollection
    {
        public int Id { get; set; }
        public int ReportId { get; set; }
        public string JsonData { get; set; }
    }
}
