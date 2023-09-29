namespace Coderr.Server.Api.Insights.Queries
{
    public class GetInsightResultApplication
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? NumberOfDevelopers { get; set; }


        public GetInsightResultIndicator[] Indicators { get; set; }
    }
}