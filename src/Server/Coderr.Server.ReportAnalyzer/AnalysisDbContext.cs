using System;
using System.Data;
using codeRR.Server.Infrastructure;
using codeRR.Server.ReportAnalyzer.Domain.Reports;
using Griffin.Data;

namespace codeRR.Server.ReportAnalyzer
{
    /// <summary>
    ///     Allows us to separate read from the write model in the future.
    /// </summary>
    public class AnalysisDbContext : IDisposable
    {
        private readonly Func<IDbConnection> _connectionFactory;
        private IDbConnection _con;
        private IAdoNetUnitOfWork _unitOfWork;


        public AnalysisDbContext(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected AnalysisDbContext(Func<IDbConnection> connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        }


        /// <summary>
        ///     Open and valid connection
        /// </summary>
        /// <exception cref="DataException">Connection failed.</exception>
        public IDbConnection Connection
        {
            get
            {
                if (_unitOfWork is OurUnitOfWork ourUnitOfWork)
                {
                    return ourUnitOfWork.Transaction.Connection;
                }
                if (_unitOfWork != null && _con == null)
                {
                    throw new NotSupportedException("Got a unit of work but not a connection");
                }

                if (_con != null)
                    return _con;
                _con = _connectionFactory();
                return _con;
            }
        }

        /// <summary>
        ///     Open and valid connection
        /// </summary>
        /// <exception cref="DataException">Connection failed.</exception>
        public IAdoNetUnitOfWork UnitOfWork => _unitOfWork
                                               ?? (_unitOfWork = new OurUnitOfWork(Connection, false));

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _con?.Dispose();
        }

        public void SaveChanges()
        {
            _unitOfWork?.SaveChanges();
        }
    }
}