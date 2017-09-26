using System.Configuration;
using System.Data;
using System.Data.Common;

namespace OneTrueError.Infrastructure
{
    /// <summary>
    ///     Generates SQL connections
    /// </summary>
    public class ConnectionFactory
    {
        /// <summary>
        ///     Creates a connection using the <c>web.config</c> connection string named <c>Db</c>.
        /// </summary>
        /// <returns>open connection</returns>
        public static IDbConnection Create()
        {
            var conStr = ConfigurationManager.ConnectionStrings["Db"];
            if (conStr == null)
                throw new ConfigurationErrorsException("Expected a <connectionString> named 'Db' in web.config");

            var provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            if (provider == null)
                throw new ConfigurationErrorsException($"Sql provider '{conStr.ProviderName}' was not found/registered.");

            var connection = provider.CreateConnection();
            connection.ConnectionString = conStr.ConnectionString;
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