using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Logs.Queries;
using Coderr.Server.Domain.Modules.Logs;
using DotNetCqs;

namespace Coderr.Server.App.Modules.Logs.Handlers
{
    internal class HasLogsHandler : IQueryHandler<HasLogs, HasLogsReply>
    {
        private readonly ILogsRepository _repository;

        public HasLogsHandler(ILogsRepository repository)
        {
            _repository = repository;
        }

        public async Task<HasLogsReply> HandleAsync(IMessageContext context, HasLogs query)
        {
            return new HasLogsReply { HasLogs = await _repository.Exists(query.IncidentId, query.ReportId) };
        }
    }
}