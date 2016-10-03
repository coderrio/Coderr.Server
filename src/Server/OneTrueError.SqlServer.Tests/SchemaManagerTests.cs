using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;

namespace OneTrueError.SqlServer.Tests
{
    public class SchemaManagerTests
    {
        public const int LatestVersion = 3;

        public SchemaManagerTests()
        {
            var sut = new SchemaManager(ConnectionFactory.CreateConnection);
            EnsureSchemaTable();
        }

        [Fact]
        public void should_report_upgradable_if_schema_version_is_less()
        {
            SetVersion(1);
        
            var sut = new SchemaManager(ConnectionFactory.CreateConnection);
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeTrue();
        }

        [Fact]
        public void should_not_report_upgradable_if_schema_version_is_same()
        {
            SetVersion(3);

            var sut = new SchemaManager(ConnectionFactory.CreateConnection);
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeFalse();
        }

        [Fact]
        public void should_report_upgradable_if_schema_table_is_missing()
        {
            DropSchemaTable();

            var sut = new SchemaManager(ConnectionFactory.CreateConnection);
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeTrue();
        }

        [Fact]
        public void should_be_able_to_upgrade_schema()
        {

            var sut = new SchemaManager(ConnectionFactory.CreateConnection);
            sut.UpgradeDatabaseSchema();


        }


        [Fact]
        public void should_be_able_to_upgrade_database()
        {
            using (var tools = new TestTools())
            {
                tools.CreateDatabase();

                var sut = new SchemaManager(tools.CreateConnection);
                sut.UpgradeDatabaseSchema();
            }
        }


        private void SetVersion(int version)
        {
            using (var con = SqlServerTools.OpenConnection())
            {
                con.ExecuteNonQuery("UPDATE DatabaseSchema SET Version = @version", new {version = version});
            }
        }

        private void EnsureSchemaTable()
        {
            using (var con = SqlServerTools.OpenConnection())
            {
                con.ExecuteNonQuery(@"
IF OBJECT_ID(N'dbo.[DatabaseSchema]', N'U') IS NULL
BEGIN
	CREATE TABLE [dbo].[DatabaseSchema] (
        [Version] int not null default 1
	);
	INSERT INTO DatabaseSchema VALUES(1);
END
");
            }
        }

        private void DropSchemaTable()
        {
            using (var con = SqlServerTools.OpenConnection())
            {
                con.ExecuteNonQuery(@"DROP TABLE [dbo].[DatabaseSchema]");
            }
        }
    }
}
