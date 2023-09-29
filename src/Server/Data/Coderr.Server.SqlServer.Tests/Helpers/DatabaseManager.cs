using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Coderr.Server.Abstractions;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Tests.Helpers
{
    /// <summary>
    ///     Purpose of this class is to create and dispose the database.
    /// </summary>
    public class DatabaseManager : IDisposable
    {
        private const string LocalDbMaster =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True";
        private const string LocalDbConStr =
            @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog={DbName};Integrated Security=True;AttachDBFilename={DbFileName};";

        private bool _deleted;
        private bool _invoked;

        public DatabaseManager(string dbName)
        {
            DbName = dbName;
            ConnectionString = LocalDbConStr;
            ConnectionString = ConnectionString
                .Replace("{DbName}", dbName)
                .Replace("{DbFileName}", GetDbFilename());
            UpdateToLatestVersion = true;
        }

        public string ConnectionString { get; }
        public string DbName { get; }

        public bool UpdateToLatestVersion { get; set; }

        public void Dispose()
        {
            if (_deleted)
                return;
            _deleted = true;
        }

        public void CreateDatabase()
        {
            var fileName = GetDbFilename();
            if (File.Exists(fileName))
            {
                return;
            }

            using (var connection = new SqlConnection(LocalDbMaster))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = $"CREATE DATABASE {DbName} ON (NAME = N'{DbName}', FILENAME = '{fileName}')";
                cmd.ExecuteNonQuery();
            }
        }

        public AdoNetUnitOfWork CreateUnitOfWork()
        {
            return new AdoNetUnitOfWork(OpenConnection(), true);
        }

        public void InitSchema()
        {
            if (_invoked)
                throw new InvalidOperationException("Already invoked.");
            _invoked = true;
            try
            {
                var tools = new SqlServerTools(OpenConnection);
                tools.CreateTables();
            }
            catch (SqlException ex)
            {
                throw new DataException($"{DbName} [{ConnectionString}] schema init failed.", ex);
            }
        }

        public IDbConnection OpenConnection()
        {
            return OpenConnection(ConnectionString);
        }
        
        private string GetDbFilename()
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return Path.Combine(dir, DbName + ".mdf");
        }
        


        private IDbConnection OpenConnection(string connectionString)
        {
            var connection = new SqlConnection { ConnectionString = connectionString };
            connection.Open();
            return connection;
        }
    }
}