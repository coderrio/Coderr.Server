using System;

namespace codeRR.Server.Infrastructure.Queueing
{
    public interface IQueueTransaction : IDisposable
    {
        void Commit();
        void Rollback();
    }
}