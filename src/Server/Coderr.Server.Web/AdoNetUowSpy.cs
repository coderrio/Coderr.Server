using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Web;
using Griffin.Data;
using log4net;

namespace codeRR.Server.Web
{
    public class AdoNetUowSpy : IAdoNetUnitOfWork
    {
        private readonly AdoNetUnitOfWork _uow;
        private ILog _logger = LogManager.GetLogger(typeof(AdoNetUowSpy));
        private static int Counter;
        private int _id;

        public AdoNetUowSpy(AdoNetUnitOfWork uow)
        {
            _id = Interlocked.Increment(ref Counter);
            Log("AdoNetUowSpy");
            _uow = uow;
        }

        private void Log(string methodName, string msg = null)
        {
            return;
            if (msg == null)
                _logger.Info(DateTime.Now.ToLongTimeString() + " UOW[" + _id + "] " + methodName);
            else
                _logger.Info(DateTime.Now.ToLongTimeString() + " UOW[" + _id + "] " + methodName + ": " + msg);
        }

        public void Dispose()
        {
            Log("Dispose");
            _uow.Dispose();
        }

        public void SaveChanges()
        {
            Log("SaveChanges");
            _uow.SaveChanges();
        }

        public IDbCommand CreateCommand()
        {
            Log("CreateCommand");
            return new WrappedCommand((DbCommand)_uow.CreateCommand(), _id);
        }

        public void Execute(string sql, object parameters)
        {
            Log("Execute", sql);
        }
    }

    public class WrappedCommand : DbCommand
    {
        private ILog _logger = LogManager.GetLogger(typeof(AdoNetUowSpy));
        private int _id;
        private DbCommand _inner;

        public WrappedCommand(DbCommand inner, int id)
        {
            _inner = inner;
            _id = id;
        }

        public override void Prepare()
        {
            _inner.Prepare();
        }

        public override string CommandText
        {
            get { return _inner.CommandText; }
            set { _inner.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get { return _inner.CommandTimeout; }
            set { _inner.CommandTimeout = value; }
        }

        public override CommandType CommandType
        {
            get { return _inner.CommandType; }
            set { _inner.CommandType = value; }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get { return _inner.UpdatedRowSource; }
            set { _inner.UpdatedRowSource = value; }
        }

        protected override DbConnection DbConnection
        {
            get { return _inner.Connection; }
            set { _inner.Connection = value; }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get { return _inner.Parameters; }
        }

        protected override DbTransaction DbTransaction
        {
            get { return _inner.Transaction; }
            set { _inner.Transaction = value; }
        }

        public override bool DesignTimeVisible
        {
            get { return _inner.DesignTimeVisible; }
            set { _inner.DesignTimeVisible = value; }
        }

        public override void Cancel()
        {
            _inner.Cancel();
        }

        protected override DbParameter CreateDbParameter()
        {
            return _inner.CreateParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            Log("ExecuteReader");
            return _inner.ExecuteReader(behavior);
        }

        private void Log(string methodName)
        {
            List<object> ps = new List<object>();
            foreach (IDataParameter p in Parameters)
            {
                ps.Add(p.ParameterName + "=" + p.Value);
            }
            _logger.InfoFormat(DateTime.Now.ToLongTimeString() + " UOW[" + _id + "]: " + methodName + "() '" + CommandText + "', params: " + string.Join(",", ps));
        }

        public override int ExecuteNonQuery()
        {
            Log("ExecuteNoQuery");
            return _inner.ExecuteNonQuery();
        }

        public override object ExecuteScalar()
        {
            Log("ExecuteScalar");
            return _inner.ExecuteScalar();
        }
    }
}