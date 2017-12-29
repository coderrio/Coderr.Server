using System.Data;
using System.Data.SqlClient;
using codeRR.Server.SqlServer.Tools;
using Griffin.Data;

namespace codeRR.Server.SqlServer.Tests
{
    internal class ConnectionFactory
    {
        public static IAdoNetUnitOfWork Create()
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConnectionStringHelper.GetConnectionString().ConnectionString
            };
            connection.Open();
            return new AdoNetUnitOfWork(connection, true);
        }

        public static IDbConnection OpenConnection()
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConnectionStringHelper.GetConnectionString().ConnectionString
            };
            connection.Open();
            return connection;
        }

        public IDbConnection Open()
        {
            return OpenConnection();
        }

        public IDbConnection Open(string connectionStringName)
        {
            return OpenConnection();
        }

        public IDbConnection TryOpen(string connectionStringName)
        {
            if (connectionStringName == "Queue")
                return null;
            return OpenConnection();
        }
    }
}