using System;
using System.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.Incidents;

namespace OneTrueError.SqlServer.Core.Incidents
{
    public class IncidentSummaryMapper : IEntityMapper<IncidentSummaryDTO>
    {
        public object Create(IDataRecord record)
        {
            return new IncidentSummaryDTO((int) record["Id"], (string) record["Description"]);
        }

        public void Map(IDataRecord source, object destination)
        {
            Map(source, (IncidentSummaryDTO) destination);
        }

        public void Map(IDataRecord source, IncidentSummaryDTO destination)
        {
            destination.ApplicationId = (int) source["ApplicationId"];
            destination.ApplicationName = (string) source["ApplicationName"];
            destination.ReportCount = (int) source["Count"];
            destination.LastUpdateAtUtc = (DateTime) source["UpdatedAtUtc"];
            destination.CreatedAtUtc = (DateTime) source["CreatedAtUtc"];
        }
    }
}