using System;
using System.Data;
using System.Data.SqlClient;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Infrastructure;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Coderr.Server.Web.Boot.Modules
{
    public class DbConnectionConfig : IAppModule
    {
        private static IConfiguration _config;

        public void Configure(ConfigurationContext context)
        {
            _config = context.Configuration;
            context.Services.AddScoped(x => OpenConnection());
            context.Services.AddScoped(x => x.GetRequiredService<IDbConnection>().BeginTransaction()); 
            context.Services.AddScoped<IAdoNetUnitOfWork>(x => new SqlServer.UnitOfWorkWithTransaction((SqlTransaction)x.GetRequiredService<IDbTransaction>()));
            context.Services.AddScoped<IAdoNetUnitOfWork>(x => new PostgreSQL.UnitOfWorkPostgreSQLWithTransaction((NpgsqlTransaction)x.GetRequiredService<IDbTransaction>()));
        }

        public IDbConnection OpenConnection()
        {
            var db = _config.GetConnectionString("Db");
            if (db == null)
            {
                throw new InvalidOperationException("Missing the connection string 'Db'.");
            }
            var con = SetupTools.DbTools.OpenConnection();
            return con;
        }

        public void Start(StartContext context)
        {
        }

        public void Stop()
        {
        }
    }
}
