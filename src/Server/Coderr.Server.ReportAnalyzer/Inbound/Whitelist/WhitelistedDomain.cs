namespace Coderr.Server.ReportAnalyzer.Inbound.Whitelist
{
    public class WhitelistedDomain
    {
        public WhitelistedDomain(int? applicationId, string domainName)
        {
            ApplicationId = applicationId;
            DomainName = domainName;
        }

        public int Id { get; private set; }

        public int? ApplicationId { get; private set; }
        public string DomainName { get; private set; }
    }
}