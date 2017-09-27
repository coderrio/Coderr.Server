using System;
using System.Messaging;

namespace codeRR.Server.Infrastructure.Queueing.Msmq
{
    public class MsmqTransactionAdapter : IQueueTransaction
    {
        private bool _haveDisposed; // To detect redundant calls

        public MsmqTransactionAdapter()
        {
            Transaction = new MessageQueueTransaction();
        }

        public MessageQueueTransaction Transaction { get; }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            Transaction.Abort();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_haveDisposed) return;

            if (disposing)
            {
                Transaction.Dispose();
            }

            _haveDisposed = true;
        }
    }
}