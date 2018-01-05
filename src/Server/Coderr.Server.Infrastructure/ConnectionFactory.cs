#if NET452
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace codeRR.Server.Infrastructure
{
    /// <summary>
    ///     Generates SQL connections
    /// </summary>
    public class DbConnectionFactory
    {
        /// <summary>
        ///     Opens a connection
        /// </summary>
        /// <returns>open connection</returns>
        public static IDbConnection Open(bool throwIfMissing)
        {
            var conString = ConfigurationManager.ConnectionStrings["Db"];
            if (conString == null)
                throw new ConfigurationErrorsException("Failed to find connectionString 'Db' in web.config");

            var hostConnectionString = Environment.GetEnvironmentVariable("coderr_ConnectionString");
            if (!string.IsNullOrEmpty(hostConnectionString))
            {
                conString = new ConnectionStringSettings(conString.Name, hostConnectionString, conString.ProviderName);
            }

            var provider = DbProviderFactories.GetFactory(conString.ProviderName);
            if (provider == null)
                throw new ConfigurationErrorsException(
                    $"SQL provider '{conString.ProviderName}' was not found/registered.");

            var connection = provider.CreateConnection();
            connection.ConnectionString = conString.ConnectionString + ";connect timeout=22;";
            try
            {
                connection.Open();
            }
            catch (DataException ex)
            {
                throw new DataException(
                    $"Failed to connect to '{conString.ConnectionString}'. See inner exception for the reason.", ex);
            }

            return connection;
        }
    }
}
#endif