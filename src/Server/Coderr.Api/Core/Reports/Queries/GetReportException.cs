namespace codeRR.Server.Api.Core.Reports.Queries
{
    /// <summary>
    ///     Partial result for <see cref="GetReportResult" />.
    /// </summary>
    public class GetReportException
    {
        /// <summary>
        ///     Assembly that the exception type is declared in.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     Base class names (without namespace). At least "Exception".
        /// </summary>
        public string[] BaseClasses { get; set; }

        /// <summary>
        ///     Typically <c>exception.ToString()</c>
        /// </summary>
        public string Everything { get; set; }

        /// <summary>
        ///     Full type name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Inner exception (or null)
        /// </summary>
        public GetReportException InnerException { get; set; }

        /// <summary>
        ///     Exception.Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Exception name (first line of the exception message)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Stack trace.
        /// </summary>
        public string StackTrace { get; set; }
    }
}