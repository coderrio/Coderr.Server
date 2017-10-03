using System;
using System.Data;
using codeRR.Server.Infrastructure;
using Griffin.Data;

namespace codeRR.Server.ReportAnalyzer
{
    /// <summary>
    ///     Allows us to separate read from the write model in the future.
    /// </summary>
    public class AnalysisDbContext : IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;
        private IDbConnection _con;
        private IAdoNetUnitOfWork _unitOfWork;

        public AnalysisDbContext(IConnectionFactory connectionFactory)
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
                if (_con != null)
                    return _con;

                _con = _connectionFactory.Open();
                return _con;
            }
        }

        /// <summary>
        ///     Open and valid connection
        /// </summary>
        /// <exception cref="DataException">Connection failed.</exception>
        public IAdoNetUnitOfWork UnitOfWork => _unitOfWork
                                               ?? (_unitOfWork = new AdoNetUnitOfWork(Connection, false));

        public void Dispose()
        {
            _unitOfWork?.Dispose();
            _con?.Dispose();
        }
    }
}