using System;
using System.Data;
using System.Data.SqlClient;
using Griffin.Data;
using log4net;

namespace Coderr.Server.SqlServer
{
    /// <summary>
    ///     Required for background jobs which uses
    /// </summary>
    public class UnitOfWorkWithTransaction : IAdoNetUnitOfWork
    {
        private ILog _logger = LogManager.GetLogger(typeof(UnitOfWorkWithTransaction));


        public UnitOfWorkWithTransaction(SqlTransaction transaction)
        {
            Transaction = transaction;
        }

        public SqlTransaction Transaction { get; private set; }

        public void Dispose()
        {
            if (Transaction == null)
                return;

            _logger.Info("Rolling back " + GetHashCode());
            var connection = Transaction.Connection;
            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;

            connection?.Dispose();
            _logger.Info("Rolled back " + GetHashCode());
        }

        public void SaveChanges()
        {
            //Already commited.
            // some scenariors requires early SaveChanges
            // to prevent dead locks. 
            // when there is time, find and eliminte the reason of the deadlocks :(
            if (Transaction == null)
                return;

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
            throw new NotSupportedException();
        }
    }
}