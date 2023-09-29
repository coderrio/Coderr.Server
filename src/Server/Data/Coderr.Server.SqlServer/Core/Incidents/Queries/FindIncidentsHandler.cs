using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Incidents;
using Coderr.Server.Api.Core.Incidents.Queries;
using Coderr.Server.Domain.Core.Incidents;
using Coderr.Server.Infrastructure.Security;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
{
    public class FindIncidentsHandler : IQueryHandler<FindIncidents, FindIncidentsResult>
    {
        private readonly IAdoNetUnitOfWork _uow;
        private string _where = "";
        private string _joins = "";

        public FindIncidentsHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        private void AppendWhere(string constraint)
        {
            if (_where == "")
                _where = " WHERE " + constraint + "\r\n";
            else
                _where += " AND " + constraint + "\r\n";
        }

        private void AppendJoin(string clause)
        {
            _joins += clause + "\r\n";
        }

        public async Task<FindIncidentsResult> HandleAsync(IMessageContext context, FindIncidents query)
        {
            using (var cmd = (DbCommand)_uow.CreateCommand())
            {
                var sqlQuery = @"SELECT {0}
                                    FROM Incidents 
                                    JOIN Applications ON (Applications.Id = Incidents.ApplicationId)";

                if (!string.IsNullOrEmpty(query.Version))
                {
                    var versionId =
                        _uow.ExecuteScalar("SELECT Id FROM ApplicationVersions WHERE Version = @version",
                            new { version = query.Version });

                    AppendJoin("JOIN IncidentVersions ON (Incidents.Id = IncidentVersions.IncidentId)");
                    AppendWhere("IncidentVersions.VersionId = @versionId");
                    cmd.AddParameter("versionId", versionId);
                }
                if (query.Tags != null && query.Tags.Length > 0)
                {
                    var ourSql = @" join IncidentTags on (Incidents.Id=IncidentTags.IncidentId AND IncidentTags.Id IN (
                                    SELECT MAX(IncidentTags.Id)
                                    FROM IncidentTags 
                                    WHERE TagName IN ({0}) 
                                    AND IncidentTags.IncidentId=Incidents.Id
                                    GROUP BY IncidentId 
                                    HAVING Count(IncidentTags.Id) = {1}
                                    ))";
                    var ps = "";
                    for (var i = 0; i < query.Tags.Length; i++)
                    {
                        ps += $"@tag{i}, ";
                        cmd.AddParameter($"@tag{i}", query.Tags[i]);
                    }

                    var sql = string.Format(ourSql, ps.Remove(ps.Length - 2, 2), query.Tags.Length);
                    AppendJoin(sql);
                }

                if (query.EnvironmentIds != null && query.EnvironmentIds.Length > 0)
                {
                    AppendJoin("JOIN IncidentEnvironments ON (Incidents.Id = IncidentEnvironments.IncidentId)");
                    AppendWhere($"IncidentEnvironments.EnvironmentId IN ({string.Join(", ", query.EnvironmentIds)})");
                }

                if (!string.IsNullOrEmpty(query.ContextCollectionPropertyValue)
                    || !string.IsNullOrEmpty(query.ContextCollectionName)
                    || !string.IsNullOrEmpty(query.ContextCollectionPropertyName))
                {
                    var where = AddContextProperty(cmd, "", "Name", "ContextName", query.ContextCollectionName);
                    where = AddContextProperty(cmd, where, "PropertyName", "ContextPropertyName", query.ContextCollectionPropertyName);
                    where = AddContextProperty(cmd, where, "Value", "ContextPropertyValue", query.ContextCollectionPropertyValue);
                    if (where.EndsWith(" AND "))
                        where = where.Substring(0, where.Length - 5);
                    var ourSql =
                        $@"with ContextSearch (IncidentId) 
                        as (
                            select distinct(IncidentId)
                            from ErrorReports
                            join ErrorReportCollectionProperties ON (ErrorReports.Id = ErrorReportCollectionProperties.ReportId)
                            WHERE {where}
                        )";
                    sqlQuery = ourSql + sqlQuery;
                    AppendJoin("join ContextSearch ON (Incidents.Id = ContextSearch.IncidentId)");
                }

                if (query.ApplicationIds != null && query.ApplicationIds.Length > 0)
                {
                    foreach (var applicationId in query.ApplicationIds)
                    {
                        if (!context.Principal.IsApplicationMember(applicationId)
                            && !context.Principal.IsSysAdmin()
                            && !context.Principal.IsApplicationAdmin(applicationId))
                            throw new UnauthorizedAccessException(
                                "You are not a member of application " + applicationId);
                    }

                    var ids = string.Join(",", query.ApplicationIds);
                    AppendWhere($"Applications.Id IN ({ids})");
                }
                else if (!context.Principal.IsSysAdmin())
                {
                    var appIds = context.Principal.Claims
                        .Where(x => x.Type == CoderrClaims.Application)
                        .Select(x => x.Value)
                        .ToArray();
                    if (!appIds.Any())
                    {
                        return new FindIncidentsResult { Items = new FindIncidentsResultItem[0] };
                    }

                    AppendWhere($"Applications.Id IN({string.Join(",", appIds)})");
                }

                if (!string.IsNullOrWhiteSpace(query.FreeText))
                {
                    AppendWhere(@"(
                                    Incidents.Id IN 
                                    (
                                        SELECT Distinct IncidentId 
                                        FROM ErrorReports 
                                        WHERE StackTrace LIKE @FreeText 
                                            OR ErrorReports.Title LIKE @FreeText
                                            OR ErrorReports.ErrorId LIKE @FreeText
                                            OR Incidents.Description LIKE @FreeText)
                                    )");
                    cmd.AddParameter("FreeText", $"%{query.FreeText}%");
                }



                _where += " AND (";
                if (query.IsIgnored)
                    _where += $"State = {(int)IncidentState.Ignored} OR ";
                if (query.IsNew)
                    _where += $"State = {(int)IncidentState.New} OR ";
                if (query.IsClosed)
                    _where += $"State = {(int)IncidentState.Closed} OR ";
                if (query.IsAssigned)
                    _where += $"State = {(int)IncidentState.Active} OR ";
                if (query.ReOpened)
                    _where += "IsReOpened = 1 OR ";


                if (_where.EndsWith("OR "))
                    _where = _where.Remove(_where.Length - 4) + ") ";
                else
                    _where = _where.Remove(_where.Length - 5);

                if (query.MinDate > DateTime.MinValue)
                {
                    AppendWhere("Incidents.LastReportAtUtc >= @minDate");
                    cmd.AddParameter("minDate", query.MinDate);
                }
                if (query.MaxDate < DateTime.MaxValue)
                {
                    AppendWhere("Incidents.LastReportAtUtc <= @maxDate");
                    cmd.AddParameter("maxDate", query.MaxDate);
                }

                if (query.AssignedToId > 0)
                {
                    AppendWhere("AssignedToId = @assignedTo");
                    cmd.AddParameter("assignedTo", query.AssignedToId);
                }



                //count first;
                sqlQuery += _joins + _where;
                cmd.CommandText = string.Format(sqlQuery, "count(Incidents.Id)");
                var count = await cmd.ExecuteScalarAsync();


                // then items
                if (query.SortType == IncidentOrder.Newest)
                {
                    if (query.SortAscending)
                        sqlQuery += " ORDER BY CreatedAtUtc";
                    else
                        sqlQuery += " ORDER BY CreatedAtUtc DESC";
                }
                else if (query.SortType == IncidentOrder.LatestReport)
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
                    var offset = (query.PageNumber - 1) * query.ItemsPerPage;
                    cmd.CommandText += $@" OFFSET {offset} ROWS FETCH NEXT {query.ItemsPerPage} ROWS ONLY";
                }
                var items = await cmd.ToListAsync<FindIncidentsResultItem>();

                return new FindIncidentsResult
                {
                    Items = items.ToArray(),
                    PageNumber = query.PageNumber,
                    PageSize = query.ItemsPerPage,
                    TotalCount = (int)count
                };
            }
        }

        protected string AddContextProperty(DbCommand cmd, string sql, string sqlColumnName, string sqlParameterName, string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains("*"))
            {
                sql += $" {sqlColumnName} LIKE @{sqlParameterName}";
                cmd.AddParameter(sqlParameterName, value.Replace("*", "%"));
            }
            else
            {
                sql += $" {sqlColumnName} = @{sqlParameterName}";
                cmd.AddParameter(sqlParameterName, value);
            }
            return sql + " AND ";
        }
    }
}