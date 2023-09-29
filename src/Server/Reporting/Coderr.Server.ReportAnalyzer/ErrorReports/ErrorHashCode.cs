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


        /// <summary>
        /// Used to be able to lookup older incidents when hashing algorithm changes (to avoid duplicates).
        /// </summary>
        public string CompabilityHashSource { get; set; }
    }
}