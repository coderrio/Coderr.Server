using System.Data.SqlClient;

namespace Coderr.IntegrationTests.Core.Tools
{
    class SqlTools
    {

        public static void GetAppKey(string dbName, int applicationId, out string appKey, out string sharedSecret)
        {
            var connection = new SqlConnection
            {
                ConnectionString =
                    $"Data Source=.;Initial Catalog={dbName};Integrated Security=True;Connect Timeout=15"
            };
            connection.Open();
            using (connection)
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 AppKey, SharedSecret FROM Applications WHERE Id = @id";
                    cmd.Parameters.AddWithValue("id", applicationId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        appKey = reader.GetString(0);
                        sharedSecret = reader.GetString(1);
                    }
                }
            }
        }
        public static void GetApiKey(string dbName, out string appKey, out string sharedSecret)
        {
            var connection = new SqlConnection
            {
                ConnectionString =
                    $"Data Source=.;Initial Catalog={dbName};Integrated Security=True;Connect Timeout=15"
            };
            connection.Open();
            using (connection)
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "SELECT TOP 1 GeneratedKey, SharedSecret FROM ApiKeys";
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        appKey = reader.GetString(0);
                        sharedSecret = reader.GetString(1);
                    }
                }
            }
        }
    }
}
