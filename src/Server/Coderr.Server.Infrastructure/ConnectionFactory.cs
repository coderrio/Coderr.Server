#if NET452
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;

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
        public static IDbConnection Open(string connectionStringName, bool throwIfMissing)
        {
            var conStr = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (conStr == null)
            {
                if (throwIfMissing)
                    throw new ConfigurationErrorsException(
                        $"Expected a <connectionString> named '{connectionStringName}' in web.config");
                return null;
            }


            var provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            if (provider == null)
                throw new ConfigurationErrorsException(
                    $"Sql provider '{conStr.ProviderName}' was not found/registered.");

            var connection = provider.CreateConnection();
            connection.ConnectionString = conStr.ConnectionString + ";connect timeout=22;";
            try
            {
                connection.Open();
            }
            catch (DataException ex)
            {
                throw new DataException(
                    $"Failed to connect to '{conStr.ConnectionString}'. See inner exception for the reason.", ex);
            }

            return connection;
        }
    }
}
#endif