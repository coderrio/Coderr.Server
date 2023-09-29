using Coderr.Server.ReportAnalyzer.Abstractions.Incidents;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    /// <summary>
    ///     Result for <see cref="IReportFilter" />.
    /// </summary>
    public enum FilterResult
    {
        /// <summary>
        ///     Process it completely (storing and analyzing).
        /// </summary>
        FullAnalyzis,

        /// <summary>
        ///     Send the <see cref="ReportAddedToIncident" /> event but do not store the report.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Typically when we reached a limit for the current incident.
        ///     </para>
        /// </remarks>
        ProcessAndDiscard,

        /// <summary>
        ///     Report should not be processed at all.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         For instance when the report is received in an environment that should be ignored (like "Development").
        ///     </para>
        /// </remarks>
        DiscardReport
    }
}