using System;
using System.Messaging;

namespace OneTrueError.Infrastructure.Queueing.Msmq
{
    public class MsmqTransactionAdapter : IQueueTransaction
    {
        private bool _haveDisposed; // To detect redundant calls
        private readonly MessageQueueTransaction _transaction;

        public MsmqTransactionAdapter()
        {
            _transaction = new MessageQueueTransaction();
        }

        public MessageQueueTransaction Transaction
        {
            get { return _transaction; }
        }

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
                _transaction.Dispose();
            }

            _haveDisposed = true;
        }
    }
}