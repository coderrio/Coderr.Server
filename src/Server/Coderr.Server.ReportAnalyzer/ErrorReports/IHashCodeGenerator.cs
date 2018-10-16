using System;
using Coderr.Server.Domain.Core.ErrorReports;

namespace Coderr.Server.ReportAnalyzer.ErrorReports
{
    public interface IHashCodeGenerator
    {
        /// <summary>
        ///     Generate a new hash code
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>hash code</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        string GenerateHashCode(ErrorReportEntity entity);
    }
}