using System.Threading.Tasks;
using Coderr.Server.Api.Core.Incidents.Queries;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Incidents.Queries
{
    internal class GetIncidentForClosePageHandler :
        IQueryHandler<GetIncidentForClosePage, GetIncidentForClosePageResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public GetIncidentForClosePageHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GetIncidentForClosePageResult> HandleAsync(IMessageContext context, GetIncidentForClosePage query)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandText = @"select Incidents.Description, 
(select count(*) from IncidentFeedback WHERE IncidentFeedback.IncidentId = Incidents.Id AND IncidentFeedback.EmailAddress is not null AND IncidentFeedback.EmailAddress <> '') as SubscriberCount
FROM Incidents
WHERE Incidents.Id = @incidentId";
                cmd.AddParameter("incidentId", query.IncidentId);
                return await cmd.FirstAsync<GetIncidentForClosePageResult>();
            }
        }
    }
}