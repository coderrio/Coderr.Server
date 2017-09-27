using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Griffin.Data;

namespace codeRR.SqlServer.Tests
{
    internal class ConnectionFactory
    {
        public static IAdoNetUnitOfWork Create()
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString
            };
            connection.Open();
            return new AdoNetUnitOfWork(connection, true);
        }

        public static IDbConnection OpenConnection()
        {
            var connection = new SqlConnection
            {
                ConnectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString
            };
            connection.Open();
            return connection;
        }
    }
}