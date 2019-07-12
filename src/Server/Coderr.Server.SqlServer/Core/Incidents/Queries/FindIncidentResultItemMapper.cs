using System;
using System.Data;
using Coderr.Server.Api.Core.Incidents.Queries;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
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
            destination.ApplicationId = (int)source["ApplicationId"];
            destination.IsReOpened = source["IsReopened"].Equals(1);
            destination.ReportCount = (int) source["ReportCount"];
            destination.CreatedAtUtc = (DateTime)source["CreatedAtUtc"];

            var value = source["UpdatedAtUtc"];
            if (!(value is DBNull))
                destination.LastUpdateAtUtc = (DateTime) value;

            value = source["LastReportAtUtc"];
            destination.LastReportReceivedAtUtc = (DateTime) (value is DBNull ? destination.LastUpdateAtUtc : value);

            value = source["AssignedAtUtc"];
            destination.AssignedAtUtc = (DateTime?)(value is DBNull ? null : value);
        }
    }
}