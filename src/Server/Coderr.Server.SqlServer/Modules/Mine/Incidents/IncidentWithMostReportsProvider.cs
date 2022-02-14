using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Modules.Mine;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Modules.Mine.Incidents
{
    [ContainerService]
    class IncidentWithMostReportsProvider : IRecommendationProvider
    {
        private readonly IAdoNetUnitOfWork _uow;

        public IncidentWithMostReportsProvider(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Recommend(RecommendIncidentContext context)
        {
            var sql = context.ApplicationId > 0
                ? $@"SELECT TOP(5) Incidents.Id IncidentId, Applications.Id as ApplicationId, Applications.Name as ApplicationName, ReportCount as Score, 'Replace' as Motivation
                                FROM Incidents 
                                JOIN Applications ON (Applications.Id = Incidents.ApplicationId)
                                WHERE State = 0
                                AND ApplicationId = {context.ApplicationId.Value}
                                ORDER BY ReportCount DESC"
                : $@"SELECT TOP(5) Incidents.Id IncidentId, Applications.Id as ApplicationId, Applications.Name as ApplicationName, ReportCount as Score, 'Replace' as Motivation
                                FROM Incidents 
                                JOIN Applications ON (Applications.Id = Incidents.ApplicationId)
                                WHERE State = 0
                                ORDER BY ReportCount DESC";

            var items = await _uow.ToListAsync<RecommendedIncident>(new MirrorMapper<RecommendedIncident>(), sql);
            if (!items.Any())
                return;

            var totalReports = (double)items.Sum(x => x.Score);
            foreach (var item in items)
            {
                var count = item.Score;
                item.Score = (int)((item.Score / totalReports) * 100);
                item.Motivation = $"Frequently reported ({count:N0} times)";
                context.Add(item, 1);
            }

        }
    }
}