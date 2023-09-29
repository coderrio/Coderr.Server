using System.Threading.Tasks;
using Coderr.Server.Api.Modules.History.Queries;
using Coderr.Server.Domain.Core.Incidents;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.App.Modules.History.Queries
{
    internal class GetIncidentStateSummaryHandler : IMessageHandler<GetIncidentStateSummary>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetIncidentStateSummaryHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(IMessageContext context, GetIncidentStateSummary message)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"select ih.State, count(*) as Count
                                    from IncidentHistory ih
                                    join IncidentVersions iv on (ih.IncidentId = iv.IncidentId)
                                    join ApplicationVersions av on (av.Id = iv.VersionId)
                                    WHERE ih.ApplicationVersion = @version
                                    AND av.ApplicationId = @appId
                                    group by ih.State
                                    ";

                cmd.AddParameter("version", message.ApplicationVersion);
                cmd.AddParameter("appId", message.ApplicationId);

                var entry = new GetIncidentStateSummaryResult();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var state = (IncidentState) reader.GetInt32(0);
                        var count = reader.GetInt32(1);
                        switch (state)
                        {
                            case IncidentState.New:
                                entry.NewCount = count;
                                break;
                            case IncidentState.ReOpened:
                                entry.ReOpenedCount = count;
                                break;
                            case IncidentState.Closed:
                                entry.ClosedCount = count;
                                break;
                        }
                    }
                }
            }
        }
    }
}