using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Modules.Logs.Commands;
using Coderr.Server.Domain.Modules.Logs;
using DotNetCqs;

namespace Coderr.Server.ReportAnalyzer.Logs
{
    internal class StoreLogEntriesHandler : IMessageHandler<StoreLogEntries>
    {
        private readonly ILogsRepository _logsRepository;

        public StoreLogEntriesHandler(ILogsRepository logsRepository)
        {
            _logsRepository = logsRepository;
        }

        public async Task HandleAsync(IMessageContext context, StoreLogEntries message)
        {
            var entries = message.Entries
                .Select(x => new LogEntry
                {
                    Exception = x.Exception,
                    Level = (LogLevel) x.Level,
                    Message = x.Message,
                    TimeStampUtc = x.TimeStampUtc
                })
                .ToList();

            await _logsRepository.Create(message.IncidentId, message.ReportId, entries);
        }
    }
}