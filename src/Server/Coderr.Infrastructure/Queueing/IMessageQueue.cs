using System;

namespace codeRR.Infrastructure.Queueing
{
    /// <summary>
    ///     Purpose of this class is to allow different queue technologies within OTE.
    /// </summary>
    public interface IMessageQueue
    {
        /// <summary>
        ///     Create a new transaction.
        /// </summary>
        /// <returns>transaction</returns>
        IQueueTransaction BeginTransaction();

        /// <summary>
        ///     Receive a new message
        /// </summary>
        /// <typeparam name="T">expected message type</typeparam>
        /// <returns>message</returns>
        /// <remarks>
        ///     <para>
        ///         Should wait until a message is available.
        ///     </para>
        /// </remarks>
        T Receive<T>();

        /// <summary>
        ///     Receive a new message
        /// </summary>
        /// <returns>message</returns>
        /// <remarks>
        ///     <para>
        ///         Should wait until a message is available.
        ///     </para>
        /// </remarks>
        object Receive();

        /// <summary>
        ///     Try receive a message
        /// </summary>
        /// <typeparam name="T">expected message type</typeparam>
        /// <param name="transaction">transaction</param>
        /// <param name="waitTimeout">amount of time to wait if queue is empty</param>
        /// <returns>message if found; otherwise <c>NULL</c></returns>
        T TryReceive<T>(IQueueTransaction transaction, TimeSpan waitTimeout);

        /// <summary>
        ///     Write a message to the queue
        /// </summary>
        /// <param name="applicationId">application that the message is for</param>
        /// <param name="message">Message</param>
        void Write(int applicationId, object message);
    }
}