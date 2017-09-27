using System;

namespace codeRR.Server.ReportAnalyzer.Domain.Reports
{
    /// <summary>
    ///     Can be used to specialize the hash code generation which is used to tell if an exception is unique or not.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For SQL Server exceptions it could use the SQL error code together with the stack trace for instance.
    ///     </para>
    /// </remarks>
    public interface IHashCodeSubGenerator
    {
        /// <summary>
        ///     Determines if this instance can generate a hashcode for the given entity.
        /// </summary>
        /// <param name="entity">entity to examine</param>
        /// <returns><c>true</c> if a hashcode can be generated; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        bool CanGenerateFrom(ErrorReportEntity entity);

        /// <summary>
        ///     Generate a new hash code
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>hashcode</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        string GenerateHashCode(ErrorReportEntity entity);
    }
}