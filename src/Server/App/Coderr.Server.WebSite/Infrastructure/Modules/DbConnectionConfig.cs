using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Coderr.Server.Abstractions;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.SqlServer;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.WebSite.Infrastructure.Modules
{
    public class DbConnectionConfig : IAppModule
    {
        public void Configure(ConfigurationContext context)
        {
            context.Services.AddScoped(x => OpenConnection());
            context.Services.AddScoped(x => x.GetRequiredService<IDbConnection>().BeginTransaction());
            context.Services.AddScoped<IAdoNetUnitOfWork>(x => new UnitOfWorkWithTransaction((DbTransaction)x.GetRequiredService<IDbTransaction>()));
        }

        internal static IDbConnection OpenConnection()
        {
            var db = HostConfig.Instance.ConnectionString;
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
