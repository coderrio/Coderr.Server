using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.Domain.Modules.History;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Modules.History
{
    [ContainerService]
    internal class HistoryRepository : IHistoryRepository
    {
        private IAdoNetUnitOfWork _unitOfWork;

        public HistoryRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(HistoryEntry entry)
        {
            await _unitOfWork.InsertAsync(entry);
        }

        public async Task<IList<HistoryEntry>> GetByIncidentId(int incidentId)
        {
            return await _unitOfWork.ToListAsync<HistoryEntry>("IncidentId = @0", incidentId);
        }
    }
}