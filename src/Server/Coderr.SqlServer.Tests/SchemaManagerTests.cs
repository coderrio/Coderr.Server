using System;
using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;

namespace codeRR.SqlServer.Tests
{
    public class SchemaManagerTests : IDisposable
    {
        public const int LatestVersion = 3;
        readonly TestTools _testTools = new TestTools();

        public SchemaManagerTests()
        {
            _testTools.CreateDatabase();
        }

     
        [Fact]
        public void Should_report_upgradable_if_schema_version_is_less()
        {
            SetVersion(1);
        
            var sut = new SchemaManager(() => _testTools.OpenConnection());
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_not_report_upgradable_if_schema_version_is_same()
        {
            _testTools.ToLatestVersion();

            var sut = new SchemaManager(() => _testTools.OpenConnection());
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeFalse();
        }

        [Fact]
        public void Should_report_upgradable_if_schema_table_is_missing()
        {

            var sut = new SchemaManager(() => _testTools.OpenConnection());
            var actual = sut.CanSchemaBeUpgraded();

            actual.Should().BeTrue();
        }

        [Fact]
        public void Should_be_able_to_upgrade_schema()
        {

            var sut = new SchemaManager(() => _testTools.OpenConnection());
            sut.UpgradeDatabaseSchema();


        }


        [Fact]
        public void Should_be_able_to_upgrade_database()
        {
            using (var tools = new TestTools())
            {
                tools.CreateDatabase();

                var sut = new SchemaManager(tools.OpenConnection);
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
            using (var con = _testTools.OpenConnection())
            {
                con.ExecuteNonQuery(@"DROP TABLE [dbo].[DatabaseSchema]");
            }
        }

        public void Dispose()
        {
            _testTools?.Dispose();
        }
    }
}
