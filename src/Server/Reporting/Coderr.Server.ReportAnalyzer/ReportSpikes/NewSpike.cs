namespace Coderr.Server.ReportAnalyzer.ReportSpikes
{
    /// <summary>
    ///     Detected a new spike
    /// </summary>
    public class NewSpike
    {
        public int ApplicationId { get; set; }
        /// <summary>
        ///     Typical report count average per day
        /// </summary>
        public int DayAverage { get; set; }

        public string ApplicationName { get; set; }

        /// <summary>
        ///     Current report count
        /// </summary>
        public int SpikeCount { get; set; }
    }
}