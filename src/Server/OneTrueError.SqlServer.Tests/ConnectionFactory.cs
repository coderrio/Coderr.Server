using System.Configuration;
using System.Data.SqlClient;
using Griffin.Data;

namespace OneTrueError.SqlServer.Tests
{
    internal class ConnectionFactory
    {
        public static IAdoNetUnitOfWork Create()
        {
            var connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            connection.Open();
            return new AdoNetUnitOfWork(connection, true);
        }
    }
}