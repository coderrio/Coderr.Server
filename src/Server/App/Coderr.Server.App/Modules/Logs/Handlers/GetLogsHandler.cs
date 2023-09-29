using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Logs.Queries;
using Coderr.Server.Domain.Modules.Logs;
using DotNetCqs;

namespace Coderr.Server.App.Modules.Logs.Handlers
{
    internal class GetLogsHandler : IQueryHandler<GetLogs, GetLogsResult>
    {
        private readonly ILogsRepository _logsRepository;

        public GetLogsHandler(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task<GetLogsResult> HandleAsync(IMessageContext context, GetLogs query)
        {
            var entries = await _logsRepository.Get(query.IncidentId, query.ReportId);
            return new GetLogsResult
            {
                Entries = entries
                    .Select(x => new GetLogsResultEntry
                    {
                        TimeStampUtc = x.TimeStampUtc,
                        Exception = x.Exception,
                        Level = (GetLogsResultEntryLevel) x.Level,
                        Message = x.Message
                    })
                    .ToArray()
            };
        }
    }
}