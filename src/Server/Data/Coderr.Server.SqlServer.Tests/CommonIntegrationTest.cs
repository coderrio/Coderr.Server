using System.Data;
using System.IO;
using System.Reflection;
using Coderr.Server.SqlServer.Partitions;
using Coderr.Server.SqlServer.Schema.Common;
using Coderr.Server.SqlServer.Schema.Oss;
using Griffin.Data.Mapper;
using Xunit.Abstractions;

namespace Coderr.Server.SqlServer.Tests
{
    public class CommonIntegrationTest : IntegrationTest
    {
        static CommonIntegrationTest()
        {
            MigrationSources.Add(("Coderr", typeof(CoderrMigrationPointer).Namespace));
            MigrationSources.Add(("Common", typeof(CommonSchemaPointer).Namespace));
            //MigrationSources.Add(("Premise", typeof(PremiseSchemaPointer).Namespace));
            MappingProvider.Scan(typeof(PartitionsRepository).Assembly);
        }

        public CommonIntegrationTest(ITestOutputHelper output) : base(output)
        {
        }

        private static void AddLiveTables(IDbConnection connection)
        {
            var sqlScript = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Coderr.Server.Common.Data.SqlServer.Tests.liveTables.sql");
            var sql = new StreamReader(sqlScript).ReadToEnd();
            using (var cmd = connection.CreateDbCommand())
            {
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
    }
}