using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.History.Queries;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Modules.History
{
    internal class GetIncidentsForStatesHandler : IQueryHandler<GetIncidentsForStates, GetIncidentsForStatesResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentsForStatesHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetIncidentsForStatesResult> HandleAsync(IMessageContext context, GetIncidentsForStates query)
        {
            var sql =
                @"select i.Id, i.Description, i.FullName, i.CreatedAtUtc, i.AssignedAtUtc, i.SolvedAtUtc, IncidentHistory.State HistoryState
                        from incidents i
                        join IncidentHistory on (i.Id= IncidentHistory.IncidentId)
                        where ApplicationVersion = @version
                        AND i.ApplicationId = @appId
                        AND IncidentHistory.State in (0, 3, 4)";
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameter("appId", query.ApplicationId);
                cmd.AddParameter("version", query.ApplicationVersion);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<GetIncidentsForStatesResultItem>();
                    while (await reader.ReadAsync())
                    {
                        var state = (IncidentState) reader["HistoryState"];
                        var item = new GetIncidentsForStatesResultItem
                        {
                            CreatedAtUtc = (DateTime) reader["CreatedAtUtc"],
                            IncidentId = (int) reader["Id"],
                            IncidentName = GetIncidentName((string) reader["Description"]),
                            IsClosed = state == IncidentState.Closed,
                            IsNew = state == IncidentState.New,
                            IsReopened = state == IncidentState.ReOpened
                        };
                        items.Add(item);
                    }

                    return new GetIncidentsForStatesResult {Items = items.ToArray()};
                }
            }
        }

        private string GetIncidentName(string description)
        {
            var pos = description.IndexOfAny(new[] {'\r', '\n'});
            if (pos != -1)
                return description.Substring(0, pos);
            return description;
        }
    }
}