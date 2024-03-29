﻿using System;
using System.Data;
using System.Data.Common;
using Coderr.Server.Abstractions;
using Griffin;
using Griffin.Data;

namespace Coderr.Server.ReportAnalyzer
{
    public class AnalysisUnitOfWork : IAdoNetUnitOfWork, IGotTransaction
    {
        private readonly bool _ownsConnection;
        private IDbConnection _connection;

        public AnalysisUnitOfWork(IDbConnection connection, bool ownsConnection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _ownsConnection = ownsConnection;
            Transaction = (DbTransaction)connection.BeginTransaction();
        }

        public DbTransaction Transaction { get; private set; }

        public void Dispose()
        {
            if (Transaction != null)
            {
                Transaction.Dispose();
                Transaction = null;
            }

            if (_connection != null && _ownsConnection)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        public void SaveChanges()
        {
            if (Transaction == null)
                return;

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
        }

        public IDbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();
            cmd.Transaction = Transaction;
            return cmd;
        }

        /// <summary>
        ///     Execute a SQL query within the transaction
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        public void Execute(string sql, object parameters)
        {
            using (var cmd = CreateCommand())
            {
                cmd.CommandText = sql;
                var dictionary = parameters.ToDictionary();
                foreach (var kvp in dictionary) cmd.AddParameter(kvp.Key, kvp.Value);
                cmd.ExecuteNonQuery();
            }
        }
    }
}