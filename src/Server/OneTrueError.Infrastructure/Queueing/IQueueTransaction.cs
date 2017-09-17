using System;

namespace OneTrueError.Infrastructure.Queueing
{
    public interface IQueueTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}