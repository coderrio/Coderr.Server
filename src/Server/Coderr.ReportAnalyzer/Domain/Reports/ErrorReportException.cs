using System;

namespace codeRR.Server.ReportAnalyzer.Domain.Reports
{
    /// <summary>
    ///     Exception for <see cref="ErrorReportEntity" />.
    /// </summary>
    public class ErrorReportException
    {
        /// <summary>
        ///     Creates a new instance of <see cref="ErrorReportException" />.
        /// </summary>
        public ErrorReportException()
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ErrorReportException" />.
        /// </summary>
        /// <param name="exception">exception that this entity represents</param>
        /// <exception cref="ArgumentNullException">exception</exception>
        public ErrorReportException(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException("exception");
            FullName = exception.GetType().FullName;
            Name = exception.GetType().Name;
            Name = exception.GetType().Namespace;
            AssemblyName = exception.GetType().Assembly.GetName().Name;
            StackTrace = exception.StackTrace;
            BaseClasses = exception.GetType().BaseType != null
                ? new[] {exception.GetType().BaseType.FullName}
                : new string[0];
            Everything = exception.ToString();
        }

        /// <summary>
        ///     Assembly containing the exception
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        ///     Class names of the base classes (minimum 'Exception').
        /// </summary>
        public string[] BaseClasses { get; set; }

        /// <summary>
        ///     Exception.ToString()
        /// </summary>
        public string Everything { get; set; }

        /// <summary>
        ///     Full type name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        ///     Inner exception if any
        /// </summary>
        public ErrorReportException InnerException { get; set; }

        /// <summary>
        ///     Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Exception class name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Namespace that the exception class was defined in
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        ///     Exception stack trace
        /// </summary>
        public string StackTrace { get; set; }
    }
}