namespace OneTrueError.Infrastructure.Queueing
{
    /// <summary>
    ///     Used to create an abstraction between the queue implementation and the usage
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Queues are important to be sure that OTE handles all the work assigned to it, even when it's run as a web site.
    ///         But since
    ///         different companies have different level of expertise and requirements we do not want to force a technology
    ///         upon them. This abstraction
    ///         therefore give them a choice to choose something that fits them.
    ///     </para>
    ///     <para>
    ///     </para>
    /// </remarks>
    public interface IMessageQueueProvider
    {
        /// <summary>
        ///     Open a queue.
        /// </summary>
        /// <param name="queueName">queue name. Currently "ReportQueue", "FeedbackQueue" or "EventQueue"</param>
        /// <returns>queue object which can be treated as a singleton.</returns>
        IMessageQueue Open(string queueName);
    }
}