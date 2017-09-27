using System;
using System.Linq;
using Griffin.Container;
using log4net;

namespace codeRR.Server.ReportAnalyzer.Domain.Reports.HashcodeGenerators
{
    /// <summary>
    ///     Generates a hash code based on URLs and status code.
    /// </summary>
    [Component]
    public class FileNotFoundHttpErrorGenerator : IHashCodeSubGenerator
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(FileNotFoundHttpErrorGenerator));


        /// <summary>
        ///     Determines if this instance can generate a hashcode for the given entity.
        /// </summary>
        /// <param name="entity">entity to examine</param>
        /// <returns><c>true</c> for <c>HttpException</c>; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        public bool CanGenerateFrom(ErrorReportEntity entity)
        {
            return entity.Exception.Name == "HttpException" ||
                   entity.Exception.BaseClasses.Any(x => x.EndsWith("HttpException"));
        }

        /// <summary>
        ///     Generate a new hash code
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>hashcode</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        public string GenerateHashCode(ErrorReportEntity entity)
        {
            var props = entity.ContextInfo.FirstOrDefault(x => x.Name == "ExceptionProperties")
                        ?? entity.ContextInfo.FirstOrDefault(x => x.Name == "Properties");
            if (props == null)
            {
                _logger.Error("Failed to find ExceptionProperties collection for entity " + entity.Id);
                return null;
            }

            string value;
            if (!props.Properties.TryGetValue("HttpCode", out value))
                return null;
            var statusCode = int.Parse(value);

            var headerProps = entity.ContextInfo.FirstOrDefault(x => x.Name == "HttpHeaders");
            if (headerProps == null)
            {
                _logger.Error("Failed to find HttpHeaders collection for entity " + entity.Id);
                return null;
            }

            var url = headerProps.Properties["Url"].ToLower();
            var pos = url.IndexOf("?");
            if (pos != -1)
                url = url.Remove(pos);

            return HashCodeUtility.GetPersistentHashCode(statusCode + "-" + url).ToString("X");
        }
    }
}