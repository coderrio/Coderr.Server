using System;
using System.Collections.Generic;

namespace codeRR.Server.ReportAnalyzer.LibContracts
{
    /// <summary>
    ///     Model used to wrap all information from an exception.
    /// </summary>
    [Serializable]
    public class ProcessReportExceptionDto
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProcessReportExceptionDto" /> class.
        /// </summary>
        public ProcessReportExceptionDto()
        {
            Properties = new Dictionary<string, string>();
        }

        /// <summary>
        ///     Assembly name (version included)
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     Exception base classes. Most specific first: <c>ArgumentOutOfRangeException</c>, <c>ArgumentException</c>,
        ///     <c>Exception</c>.
        /// </summary>
        public string[] BaseClasses { get; set; }

        /// <summary>
        ///     Everything (<c>exception.ToString()</c>)
        /// </summary>
        public string Everything { get; set; }

        /// <summary>
        ///     Full type name (namespace + class name)
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Inner exception (if any; otherwise <c>null</c>).
        /// </summary>
        public ProcessReportExceptionDto InnerExceptionDto { get; set; }

        /// <summary>
        ///     Exception message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Type name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Namespace that the exception is in
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     All properties (public and private)
        /// </summary>
        public IDictionary<string, string> Properties { get; set; }

        /// <summary>
        ///     Stack trace, line numbers included if your app also distributes the PDB files.
        /// </summary>
        public string StackTrace { get; set; }
    }
}