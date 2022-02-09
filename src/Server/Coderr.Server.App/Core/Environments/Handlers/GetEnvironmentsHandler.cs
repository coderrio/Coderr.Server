using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Environments.Queries;
using DotNetCqs;

namespace Coderr.Server.App.Core.Environments.Handlers
{
    public class GetEnvironmentsHandler : IQueryHandler<GetEnvironments, GetEnvironmentsResult>
    {
        private readonly IEnvironmentRepository _repository;

        public GetEnvironmentsHandler(IEnvironmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetEnvironmentsResult> HandleAsync(IMessageContext context, GetEnvironments query)
        {
            var items = (await _repository.ListForApplication(query.ApplicationId))
                .Select(x => new GetEnvironmentsResultItem { Name = x.Name, Id = x.EnvironmentId, DeleteIncidents = x.DeleteIncidents })
                .ToArray();

            return new GetEnvironmentsResult { Items = items };
        }
    }
}