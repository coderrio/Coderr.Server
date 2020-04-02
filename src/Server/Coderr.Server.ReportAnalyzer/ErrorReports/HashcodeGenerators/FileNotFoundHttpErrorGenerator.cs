using System;
using System.Linq;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Core.ErrorReports;
using log4net;

namespace Coderr.Server.ReportAnalyzer.ErrorReports.HashcodeGenerators
{
    /// <summary>
    ///     Generates a hash code based on URLs and status code.
    /// </summary>
    [ContainerService]
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
        public ErrorHashCode GenerateHashCode(ErrorReportEntity entity)
        {
            string requestUrl = null;
            var httpCode = 0;

            foreach (var collection in entity.ContextCollections)
            {
                if (collection.Properties.TryGetValue("HttpCode", out var value))
                {
                    int.TryParse(value, out httpCode);

                }

                if (!collection.Properties.TryGetValue("RequestUrl", out requestUrl))
                    collection.Properties.TryGetValue("Url", out requestUrl);

            }

            if (httpCode == 0 || string.IsNullOrWhiteSpace(requestUrl))
                return null;

            var pos = requestUrl.IndexOf("?");
            if (pos != -1)
                requestUrl = requestUrl.Remove(pos);

            return new ErrorHashCode
            {
                HashCode = HashCodeUtility.GetPersistentHashCode($"{httpCode}-{requestUrl}").ToString("X"),
                CollisionIdentifier = $"{httpCode}-{requestUrl}"
            };
        }
    }
}