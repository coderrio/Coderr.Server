using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data;

namespace OneTrueError.SqlServer.Tests
{
    class ConnectionFactory
    {
        public static IAdoNetUnitOfWork Create()
        {
            var connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            connection.Open();
            return new AdoNetUnitOfWork(connection, true);
        }

        public static IDbConnection CreateConnection()
        {
            var connection = new SqlConnection();
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["Db"].ConnectionString;
            connection.Open();
            return connection;
        }
    }
}
