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
            context.Services.AddScoped<IAdoNetUnitOfWork>(x =>
            {
                var con = OpenConnection();
                var transaction = con.BeginTransaction();
                return new UnitOfWorkWithTransaction((SqlTransaction)transaction);
            });
        }

        public IDbConnection OpenConnection()
        {
            var db = _config.GetConnectionString("Db");
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
