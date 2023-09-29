using DotNetCqs.Queues;

namespace Coderr.Server.Infrastructure.Messaging
{
    /// <summary>
    ///     Register a queue.
    /// </summary>
    public interface IRouteRegistrar
    {
        /// <summary>
        ///     Queue for application (i.e. business specific) messages (all but the ReportAnalyzer namespaces).
        /// </summary>
        /// <param name="queue"></param>
        void RegisterAppQueue(IMessageQueue queue);

        /// <summary>
        ///     Queue for the report analyzer pipeline (the report analyzer namespaces).
        /// </summary>
        /// <param name="queue"></param>
        void RegisterReportQueue(IMessageQueue queue);
    }
}