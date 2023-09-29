using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.Logs
{
    public interface ILogsRepository
    {
        Task<bool> Exists(int incidentId, int? reportId);
        Task<IReadOnlyList<LogEntry>> Get(int incidentId, int? reportId);
        Task Create(int incidentId, int reportId, IReadOnlyList<LogEntry> entries);
    }
}