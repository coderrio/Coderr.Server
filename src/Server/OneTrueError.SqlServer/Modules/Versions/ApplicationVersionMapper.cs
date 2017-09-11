using Griffin.Data.Mapper;
using OneTrueError.App.Modules.Versions;
using OneTrueError.App.Modules.Versions.Events;

namespace OneTrueError.SqlServer.Modules.Versions
{
    public class ApplicationVersionMapper : CrudEntityMapper<ApplicationVersion>
    {
        public ApplicationVersionMapper() : base("ApplicationVersions")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.ReceivedFirstReportAtUtc)
                .ColumnName("FirstReportDate");
            Property(x => x.ReceivedLastReportAtUtc)
                .ColumnName("LastReportDate");
        }
    }
}