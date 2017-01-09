using System;

namespace OneTrueError.Infrastructure.Queueing
{
    /// <summary>
    ///     Purpose of this class is to allow different queue technologies within OTE.
    /// </summary>
    public interface IMessageQueue
    {
        IQueueTransaction BeginTransaction();

        //T Receive<T>(IQueueTransaction transaction);
        T Receive<T>();
        object Receive();
        T TryReceive<T>(IQueueTransaction transaction, TimeSpan waitTimeout);
        void Write(int applicationId, object message);
        //T TryReceive<T>(TimeSpan waitTimeout);
    }
}