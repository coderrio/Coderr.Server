using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Incidents;
using codeRR.Server.Api.Core.Incidents.Queries;
using codeRR.Server.App.Core.Incidents;
using codeRR.Server.Infrastructure.Security;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Incidents.Queries
{
    [Component]
    public class FindIncidentsHandler : IQueryHandler<FindIncidents, FindIncidentsResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public FindIncidentsHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<FindIncidentsResult> HandleAsync(IMessageContext context, FindIncidents query)
        {
            using (var cmd = (DbCommand) _uow.CreateCommand())
            {
                var sqlQuery = @"SELECT {0}
                                    FROM Incidents 
                                    JOIN Applications ON (Applications.Id = Incidents.ApplicationId)";

                var startWord = " WHERE ";
                if (!string.IsNullOrEmpty(query.Version))
                {
                    var versionId =
                        _uow.ExecuteScalar("SELECT Id FROM ApplicationVersions WHERE Version = @version",
                            new {version = query.Version});

                    sqlQuery += " JOIN IncidentVersions ON (Incidents.Id = IncidentVersions.IncidentId)" +
                                " WHERE IncidentVersions.VersionId = @versionId";
                    cmd.AddParameter("versionId", versionId);
                    startWord = " AND ";
                }

                if (query.ApplicationId > 0)
                {
                    if (!context.Principal.IsApplicationMember(query.ApplicationId)
                        && !context.Principal.IsSysAdmin()
                        && !context.Principal.IsApplicationAdmin(query.ApplicationId))
                        throw new UnauthorizedAccessException(
                            "You are not a member of application " + query.ApplicationId);

                    sqlQuery += $" {startWord} Applications.Id = @id";
                    cmd.AddParameter("id", query.ApplicationId);
                }
                else if (!context.Principal.IsSysAdmin())
                {
                    var appIds = context.Principal.Claims
                        .Where(x => x.Type == CoderrClaims.Application)
                        .Select(x => x.Value)
                        .ToArray();
                    sqlQuery += $" {startWord} Applications.Id IN({string.Join(",", appIds)})";
                }

                if (query.FreeText != null)
                {
                    sqlQuery += @" AND (
                                    Incidents.Id IN 
                                    (
                                        SELECT Distinct IncidentId 
                                        FROM ErrorReports 
                                        WHERE StackTrace LIKE @FreeText 
                                            OR ErrorReports.Title LIKE @FreeText
                                            OR Incidents.Description LIKE @FreeText)
                                    )";
                    cmd.AddParameter("FreeText", $"%{query.FreeText}%");
                }

                

                sqlQuery += " AND (";
                if (query.IsIgnored)
                    sqlQuery += $"State = {(int)IncidentState.Ignored} OR ";
                if (query.IsNew)
                    sqlQuery += $"State = {(int)IncidentState.New} OR ";
                if (query.IsClosed)
                    sqlQuery += $"State = {(int)IncidentState.Closed} OR ";
                if (query.IsAssigned)
                    sqlQuery += $"State = {(int)IncidentState.Active} OR ";
                if (query.ReOpened)
                    sqlQuery += "IsReOpened = 1 OR ";

                if (sqlQuery.EndsWith("OR "))
                    sqlQuery = sqlQuery.Remove(sqlQuery.Length - 4) + ") ";
                else
                    sqlQuery = sqlQuery.Remove(sqlQuery.Length - 5);

                if (query.MinDate > DateTime.MinValue)
                {
                    sqlQuery += " AND Incidents.LastReportAtUtc >= @minDate";
                    cmd.AddParameter("minDate", query.MinDate);
                }
                if (query.MaxDate < DateTime.MaxValue)
                {
                    sqlQuery += " AND Incidents.LastReportAtUtc <= @maxDate";
                    cmd.AddParameter("maxDate", query.MaxDate);
                }

                //count first;
                cmd.CommandText = string.Format(sqlQuery, "count(Incidents.Id)");
                var count = await cmd.ExecuteScalarAsync();


                // then items
                if (query.SortType == IncidentOrder.Newest)
                {
                    if (query.SortAscending)
                        sqlQuery += " ORDER BY LastReportAtUtc";
                    else
                        sqlQuery += " ORDER BY LastReportAtUtc DESC";
                }
                else if (query.SortType == IncidentOrder.MostReports)
                {
                    if (query.SortAscending)
                        sqlQuery += " ORDER BY ReportCount";
                    else
                        sqlQuery += " ORDER BY ReportCount DESC";
                }
                cmd.CommandText = string.Format(sqlQuery,
                    "Incidents.*, Applications.Id as ApplicationId, Applications.Name as ApplicationName");
                if (query.PageNumber > 0)
                {
                    var offset = (query.PageNumber - 1)*query.ItemsPerPage;
                    cmd.CommandText += string.Format(@" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset,
                        query.ItemsPerPage);
                }
                var items = await cmd.ToListAsync<FindIncidentsResultItem>();

                return new FindIncidentsResult
                {
                    Items = items.ToArray(),
                    PageNumber = query.PageNumber,
                    PageSize = query.ItemsPerPage,
                    TotalCount = (int) count
                };
            }
        }
    }
}