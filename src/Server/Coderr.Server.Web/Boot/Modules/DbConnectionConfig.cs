using System;
using System.Data;
using System.Data.SqlClient;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.SqlServer;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

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
            context.Services.AddScoped<IAdoNetUnitOfWork>(x => new UnitOfWorkWithTransaction((SqlTransaction)x.GetRequiredService<IDbTransaction>()));
        }

        public IDbConnection OpenConnection()
        {
            var db = _config.GetConnectionString("Db");
            if (db == null)
            {
                throw new InvalidOperationException("Missing the connection string 'Db'.");
            }
            var con = new SqlConnection(db);
            con.Open();
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
