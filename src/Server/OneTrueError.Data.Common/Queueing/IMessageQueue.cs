using System;

namespace OneTrueError.Infrastructure.Queueing
{
    /// <summary>
    /// Purpose of this class is to allow different queue technologies within OTE.
    /// </summary>
    public interface IMessageQueue
    {
        void Write(int applicationId, object message);

        IQueueTransaction BeginTransaction();

        //T Receive<T>(IQueueTransaction transaction);
        T Receive<T>();
        object Receive();
        T TryReceive<T>(IQueueTransaction transaction, TimeSpan waitTimeout);
        //T TryReceive<T>(TimeSpan waitTimeout);
    }
}
