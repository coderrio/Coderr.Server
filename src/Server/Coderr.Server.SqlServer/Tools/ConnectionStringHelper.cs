using System;
using System.Configuration;

namespace codeRR.Server.SqlServer.Tools
{
    public static class ConnectionStringHelper
    {
        public static ConnectionStringSettings GetConnectionString()
        {
            var connectionStringName = ConfigurationManager.AppSettings["ConnectionStringName"] ?? "Db";

            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName];

            var environmentConnectionString = Environment.GetEnvironmentVariable("coderr_ConnectionString");
            if (!string.IsNullOrEmpty(environmentConnectionString))
            {
                connectionString = new ConnectionStringSettings(connectionStringName, environmentConnectionString, connectionString.ProviderName);
            }

            return connectionString;
        }
    }
}
