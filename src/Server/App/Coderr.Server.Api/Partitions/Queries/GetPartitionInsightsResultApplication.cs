namespace Coderr.Server.Api.Partitions.Queries
{
    public class GetPartitionInsightsResultApplication
    {
        public GetPartitionInsightsResultApplication(int applicationId)
        {
            ApplicationId = applicationId;
        }

        public int ApplicationId { get; private set; }
        public GetPartitionInsightsResultIndicator[] Indicators { get; set; }
    }
}