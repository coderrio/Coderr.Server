using Griffin.Data.Mapper;
using codeRR.App.Modules.Versions;
using codeRR.App.Modules.Versions.Events;

namespace codeRR.SqlServer.Modules.Versions
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