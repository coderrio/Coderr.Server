using System;
using System.Data;
using System.Data.SqlClient;
using Griffin.Data;

namespace Coderr.Server.SqlServer
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
            var connection = Transaction.Connection;
            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
            connection.Dispose();
        }

        public void SaveChanges()
        {
            var connection = Transaction.Connection;
            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
            connection.Dispose();
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