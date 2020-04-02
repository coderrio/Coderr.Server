namespace Coderr.Server.ReportAnalyzer.ErrorReports
{
    public class ErrorHashCode
    {
        /// <summary>
        /// Hashcode to use
        /// </summary>
        public string HashCode { get; set; }

        /// <summary>
        /// Used when two incidents have the same hash code to be able to separate them.
        /// </summary>
        public string CollisionIdentifier { get; set; }
    }
}