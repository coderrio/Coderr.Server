using Coderr.Server.Domain.Core.ErrorReports;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Reports
{
    class ReportMappingMapper : CrudEntityMapper<ReportMapping>
    {
        public ReportMappingMapper() : base("IncidentReports")
        {
            Property(x => x.Id)
                .PrimaryKey(true);
            Property(x => x.ApplicationId)
                .NotForCrud();
        }
    }
}
