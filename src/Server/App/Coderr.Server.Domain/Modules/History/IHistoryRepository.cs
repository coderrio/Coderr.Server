using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coderr.Server.Domain.Modules.History
{
    public interface IHistoryRepository
    {
        Task CreateAsync(HistoryEntry entry);
        Task<IList<HistoryEntry>> GetByIncidentId(int incidentId);
    }
}
