using Griffin.Data.Mapper;
using codeRR.Api.Core.Reports;
using codeRR.SqlServer.Tools;

namespace codeRR.SqlServer.Core.Reports
{
    public class ErrorReportDtoMapper : CrudEntityMapper<ReportDTO>
    {
        public ErrorReportDtoMapper()
            : base("ErrorReports")
        {
            Property(x => x.ContextCollections)
                .ColumnName("ContextInfo")
                .ToColumnValue(EntitySerializer.Serialize)
                .ToPropertyValue(colValue => EntitySerializer.Deserialize<ContextCollectionDTO[]>((string) colValue));

            Property(x => x.ReportVersion).Ignore();

            Property(x => x.Exception)
                .ToPropertyValue(EntitySerializer.Deserialize<ReportExeptionDTO>)
                .ToColumnValue(EntitySerializer.Serialize);

            Property(x => x.ReportId)
                .ColumnName("ErrorId");
        }
    }
}