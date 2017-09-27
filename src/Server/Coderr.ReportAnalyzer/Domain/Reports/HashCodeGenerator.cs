using System;
using System.Text.RegularExpressions;
using Griffin.Container;

namespace codeRR.Server.ReportAnalyzer.Domain.Reports
{
    /// <summary>
    ///     Used to generate hash codes for incoming error reports
    /// </summary>
    /// <remarks>
    ///     This new generator uses context information provided in exceptions to generate the hash code. For instance HTTP 404
    ///     exception
    ///     are based on the URI and not exception information
    /// </remarks>
    [Component]
    public class HashCodeGenerator
    {
        private const string RemoveLineNumbersRegEx = @"^(.*)(:[\w]+ [\d]+)";
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        ///     Creates a new instance of <see cref="HashCodeGenerator" />.
        /// </summary>
        /// <param name="serviceLocator">Used to resolve all <see cref="IHashCodeSubGenerator" />.</param>
        public HashCodeGenerator(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        /// <summary>
        ///     Generate a new hash code
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>hash code</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        public string GenerateHashCode(ErrorReportEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            foreach (var generator in _serviceLocator.ResolveAll<IHashCodeSubGenerator>())
            {
                if (generator.CanGenerateFrom(entity))
                {
                    // forgiving ones so that we can get the report and process it with a default hash code instead.
                    var code = generator.GenerateHashCode(entity);
                    if (code != null)
                        return code;
                }
            }

            return DefaultCreateHashCode(entity);
        }

        /// <summary>
        ///     Method that will be invoked if no implementations of <see cref="IHashCodeSubGenerator" /> generates an hash code.
        /// </summary>
        /// <param name="report">received report</param>
        /// <returns>hash code</returns>
        /// <exception cref="ArgumentNullException">report</exception>
        protected virtual string DefaultCreateHashCode(ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException("report");

            //TODO: Ta bort radnummers stripparen
            var hashSource = report.Exception.FullName + "\r\n";
            hashSource += StripLineNumbers(report.Exception.StackTrace ?? "");

            var hash = 23;
            foreach (var c in hashSource)
            {
                hash = hash*31 + c;
            }
            return hash.ToString("X");
        }

        internal static string StripLineNumbers(string stacktrace)
        {
            var re = new Regex(RemoveLineNumbersRegEx, RegexOptions.Multiline);
            return re.Replace(stacktrace, "$1", 1000);
        }
    }
}