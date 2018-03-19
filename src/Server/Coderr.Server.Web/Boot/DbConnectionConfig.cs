using System.Data;
using System.Data.SqlClient;
using Coderr.Server.Infrastructure.Boot;
using Griffin.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Coderr.Server.Web2.Boot
{
    public class DbConnectionConfig : ISystemModule
    {
        private static IConfiguration _config;

        public void Configure(ConfigurationContext context)
        {
            _config = context.Configuration;
            context.Services.AddScoped<IAdoNetUnitOfWork>(x =>
            {
                var con = OpenConnection();
                return new AdoNetUnitOfWork(con, true);
            });
        }

        public static IDbConnection OpenConnection()
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
