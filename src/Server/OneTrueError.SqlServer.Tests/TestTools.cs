using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using OneTrueError.Infrastructure;

namespace OneTrueError.SqlServer.Tests
{
    public class TestTools : IDisposable
    {
        private string _dbName;

        public void CreateDatabase()
        {
            using (var con = ConnectionFactory.CreateConnection())
            {
                _dbName = "T" + Guid.NewGuid().ToString("N");
                con.ExecuteNonQuery("CREATE Database " + _dbName);
                con.ChangeDatabase(_dbName);
                var schemaTool = new SchemaManager(() => con);
                schemaTool.CreateInitialStructure();
            }
        }

        public IDbConnection CreateConnection()
        {
            var connection = ConnectionFactory.CreateConnection();
            connection.ChangeDatabase(_dbName);
            return connection;
        }

        public void Dispose()
        {
            if (_dbName == null)
                return;

            using (var con = ConnectionFactory.CreateConnection())
            {
                var sql =
                    string.Format("alter database {0} set single_user with rollback immediate; DROP Database {0}",
                        _dbName);
                con.ExecuteNonQuery(sql);
            }
        }
    }
}
