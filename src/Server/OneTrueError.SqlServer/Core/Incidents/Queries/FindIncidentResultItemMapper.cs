using System;
using System.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Incidents.Queries;

namespace OneTrueError.SqlServer.Core.Incidents.Queries
{
    public class FindIncidentResultItemMapper : IEntityMapper<FindIncidentResultItem>
    {
        public object Create(IDataRecord record)
        {
            return new FindIncidentResultItem((int) record["Id"], (string) record["Description"]);
        }

        public void Map(IDataRecord source, object destination)
        {
            Map(source, (FindIncidentResultItem) destination);
        }

        public void Map(IDataRecord source, FindIncidentResultItem destination)
        {
            destination.ApplicationName = (string) source["ApplicationName"];
            destination.ApplicationId = source["ApplicationId"].ToString();
            destination.IsReOpened = source["IsReopened"].Equals(1);
            destination.ReportCount = (int) source["ReportCount"];
            destination.LastUpdateAtUtc = (DateTime) source["UpdatedAtUtc"];
            destination.CreatedAtUtc = (DateTime) source["CreatedAtUtc"];
        }
    }
}