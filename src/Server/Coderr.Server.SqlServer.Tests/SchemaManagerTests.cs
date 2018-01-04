using System;
using codeRR.Server.SqlServer.Tests.Helpers;
using FluentAssertions;
using Griffin.Data.Mapper;
using Xunit;

namespace codeRR.Server.SqlServer.Tests
{
    public class SchemaManagerTests
    {
        public const int LatestVersion = 3;

        [Fact]
        public void Should_report_upgradable_if_schema_version_is_less()
        {
            using (var dbMgr = new DatabaseManager())
            {
                dbMgr.UpdateToLatestVestion = false;
                dbMgr.CreateEmptyDatabase();
                dbMgr.InitSchema();
                dbMgr.UpdateSchema(1);

                var sut = new SchemaManager(() => dbMgr.OpenConnection());
                var actual = sut.CanSchemaBeUpgraded();

                actual.Should().BeTrue();
            }

        }

        [Fact]
        public void Should_not_report_upgradable_if_schema_version_is_same()
        {
            using (var dbMgr = new DatabaseManager())
            {
                dbMgr.UpdateToLatestVestion = false;
                dbMgr.CreateEmptyDatabase();
                dbMgr.InitSchema();
                dbMgr.UpdateSchema(-1);

                var sut = new SchemaManager(() => dbMgr.OpenConnection());
                var actual = sut.CanSchemaBeUpgraded();

                actual.Should().BeFalse();
            }
        }

        [Fact]
        public void Should_report_upgradable_if_schema_table_is_missing()
        {
            using (var dbMgr = new DatabaseManager())
            {
                dbMgr.UpdateToLatestVestion = false;
                dbMgr.CreateEmptyDatabase();

                var sut = new SchemaManager(() => dbMgr.OpenConnection());
                var actual = sut.CanSchemaBeUpgraded();

                actual.Should().BeTrue();
            }
        }

        [Fact]
        public void Should_be_able_to_upgrade_schema()
        {
            using (var dbMgr = new DatabaseManager())
            {
                dbMgr.UpdateToLatestVestion = false;
                dbMgr.CreateEmptyDatabase();
                dbMgr.InitSchema();

                var sut = new SchemaManager(() => dbMgr.OpenConnection());
                sut.UpgradeDatabaseSchema();

            }
        }

    }
}
