namespace Coderr.Server.PostgreSQL.ReportAnalyzer.Jobs
{
    public class ErrorReportContextCollectionProperty
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        /// <summary>
        ///     Context collection name
        /// </summary>
        public string CollectionName { get; set; }

        public string PropertyName { get; set; }

        public string Value { get; set; }
    }

}
