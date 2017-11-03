using System;
using System.Data;
using codeRR.Server.Api.Core.Incidents.Queries;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    public class FindIncidentResultItemMapper : IEntityMapper<FindIncidentsResultItem>
    {
        public object Create(IDataRecord record)
        {
            return new FindIncidentsResultItem((int) record["Id"], (string) record["Description"]);
        }

        public void Map(IDataRecord source, object destination)
        {
            Map(source, (FindIncidentsResultItem) destination);
        }

        public void Map(IDataRecord source, FindIncidentsResultItem destination)
        {
            destination.ApplicationName = (string) source["ApplicationName"];
            destination.ApplicationId = source["ApplicationId"].ToString();
            destination.IsReOpened = source["IsReopened"].Equals(1);
            destination.ReportCount = (int) source["ReportCount"];
            destination.LastUpdateAtUtc = (DateTime) source["UpdatedAtUtc"];

            var value = source["LastReportAtUtc"];
            destination.LastReportReceivedAtUtc = (DateTime) (value is DBNull ? destination.LastUpdateAtUtc : value);

            destination.CreatedAtUtc = (DateTime) source["CreatedAtUtc"];
        }
    }
}