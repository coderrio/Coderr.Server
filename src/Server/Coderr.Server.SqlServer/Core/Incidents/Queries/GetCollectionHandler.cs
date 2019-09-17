using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;
using Griffin.Data;
using log4net;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
{
    internal class GetCollectionHandler : IQueryHandler<GetCollection, GetCollectionResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private ILog _logger = LogManager.GetLogger(typeof(GetCollectionHandler));

        public GetCollectionHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCollectionResult> HandleAsync(IMessageContext context, GetCollection query)
        {
            if (query.MaxNumberOfCollections == 0)
                query.MaxNumberOfCollections = 1;

            var sql = @"WITH ReportsWithCollection (ErrorReportId)
                        AS
                        (
                            select distinct TOP(10) ErrorReports.Id
                            FROM ErrorReports
                            JOIN ErrorReportCollectionProperties ep ON (ep.ReportId = ErrorReports.Id)
                            WHERE ep.Name = @collectionName
                            AND ErrorReports.IncidentId=@incidentId
                        )

                        select erp.PropertyName, erp.Value, ErrorReports.CreatedAtUtc, ErrorReports.Id ReportId
                        from ErrorReportCollectionProperties erp
                        join ReportsWithCollection rc on (erp.ReportId = rc.ErrorReportId)
                        join ErrorReports on (ErrorReports.ID = rc.ErrorReportId)
                        WHERE erp.Name = @collectionName";

            var items = new List<GetCollectionResultItem>();
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("incidentId", query.IncidentId);
                cmd.AddParameter("collectionName", query.CollectionName);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    GetCollectionResultItem item = null;
                    var lastReportId = 0;
                    while (reader.Read())
                    {
                        var reportId = (int)reader["ReportId"];
                        if (reportId != lastReportId || item == null)
                        {
                            item = new GetCollectionResultItem
                            {
                                ReportId = (int)reader["ReportId"],
                                ReportDate = (DateTime)reader["CreatedAtUtc"],
                                Properties = new Dictionary<string, string>()
                            };
                            items.Add(item);
                        }

                        lastReportId = reportId;
                        var key = (string)reader["PropertyName"];
                        var value = (string)reader["Value"];
                        if (item.Properties.ContainsKey(key))
                        {
                            _logger.Info(
                                $"Report {reportId} have value for key {key} current: {item.Properties[key]} new: {value}.");
                        }
                        else
                            item.Properties.Add(key, value);
                    }
                }
            }

            return new GetCollectionResult
            {
                Items = items.ToArray()
            };
        }
    }
}
