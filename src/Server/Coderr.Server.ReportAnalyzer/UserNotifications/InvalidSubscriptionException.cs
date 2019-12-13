using System;

namespace Coderr.Server.ReportAnalyzer.UserNotifications
{
    /// <summary>
    ///     Browser subscription is not valid (remove it).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         TODO: Notify the user that it was removed.
    ///     </para>
    /// </remarks>
    public class InvalidSubscriptionException : Exception
    {
        /// <inheritdoc />
        public InvalidSubscriptionException(string errorMessage, Exception inner)
            : base(errorMessage, inner)
        {
        }
    }
}