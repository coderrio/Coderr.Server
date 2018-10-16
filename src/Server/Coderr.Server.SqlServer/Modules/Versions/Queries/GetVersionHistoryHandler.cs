using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Versions.Queries;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Modules.Versions.Queries
{
    public class GetVersionHistoryHandler : IQueryHandler<GetVersionHistory, GetVersionHistoryResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetVersionHistoryHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<GetVersionHistoryResult> HandleAsync(IMessageContext context, GetVersionHistory query)
        {
            var sql =
                @"select avm.YearMonth, avm.IncidentCount, avm.ReportCount, LastUpdateAtUtc, av.Version
                  from [ApplicationVersionMonths] avm
                  JOIN ApplicationVersions av ON (av.Id=avm.VersionId)
                  WHERE av.ApplicationId = @appId
                  ORDER BY YearMonth, ApplicationId, av.Version";

            var first = DateTime.MinValue;
            var last = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.AddParameter("appId", query.ApplicationId);
                if (query.FromDate != null)
                {
                    sql += " AND YearMonth >= @from";
                    cmd.AddParameter("from", query.FromDate.Value);
                    first = query.FromDate.Value;
                }

                if (query.ToDate != null)
                {
                    sql += " AND YearMonth <= @to";
                    cmd.AddParameter("to", query.ToDate.Value);
                    last = query.ToDate.Value;
                }

                cmd.CommandText = sql;
                var versions = new Versions();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var month = (DateTime) reader["YearMonth"];
                        if (first == DateTime.MinValue)
                            first = month;
                        var incindentCount = (int) reader["IncidentCount"];
                        var reportCount = (int) reader["ReportCount"];
                        var version = (string) reader["Version"];

                        versions.AddCounts(version, month, incindentCount, reportCount);
                    }
                }

                if (versions.IsEmpty)
                    return new GetVersionHistoryResult
                    {
                        Dates = new string[0],
                        IncidentCounts = new GetVersionHistoryResultSet[0],
                        ReportCounts = new GetVersionHistoryResultSet[0]
                    };


                versions.PadMonths(first, last);

                var result = new GetVersionHistoryResult
                {
                    Dates = versions.GetDates(),
                    IncidentCounts = versions.BuildIncidentSeries(),
                    ReportCounts = versions.BuildReportSeries()
                };
                return result;
            }
        }
    }
}