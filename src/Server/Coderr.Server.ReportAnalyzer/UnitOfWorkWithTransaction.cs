using System;
using System.Data;
using System.Data.SqlClient;
using Griffin.Data;

namespace Coderr.Server.ReportAnalyzer
{
    /// <summary>
    ///     Required for background jobs which uses
    /// </summary>
    public class UnitOfWorkWithTransaction : IAdoNetUnitOfWork
    {
        public UnitOfWorkWithTransaction(SqlTransaction transaction)
        {
            Transaction = transaction;
        }

        public SqlTransaction Transaction { get; private set; }

        public void Dispose()
        {
            if (Transaction == null)
                return;
            Transaction.Rollback();
            Transaction.Connection.Dispose();
            Transaction.Dispose();
            Transaction = null;
        }

        public void SaveChanges()
        {
            Transaction.Commit();
            Transaction.Connection.Dispose();
            Transaction.Dispose();
            Transaction = null;
        }

        public IDbCommand CreateCommand()
        {
            var cmd = Transaction.Connection.CreateCommand();
            cmd.Transaction = Transaction;
            return cmd;
        }

        public void Execute(string sql, object parameters)
        {
            throw new NotImplementedException();
        }
    }
}