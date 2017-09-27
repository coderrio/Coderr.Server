using System;

namespace codeRR.Infrastructure.Queueing
{
    public interface IQueueTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}