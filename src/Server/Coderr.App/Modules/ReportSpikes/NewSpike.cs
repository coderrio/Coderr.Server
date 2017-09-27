namespace codeRR.Server.App.Modules.ReportSpikes
{
    /// <summary>
    ///     Detected a new spike
    /// </summary>
    public class NewSpike
    {
        /// <summary>
        ///     Typical report count average per day
        /// </summary>
        public int DayAverage { get; set; }

        /// <summary>
        ///     Current report count
        /// </summary>
        public int SpikeCount { get; set; }
    }
}