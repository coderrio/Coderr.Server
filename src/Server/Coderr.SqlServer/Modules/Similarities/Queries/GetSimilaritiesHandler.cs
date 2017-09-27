using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using log4net;
using codeRR.Api.Modules.ContextData.Queries;
using codeRR.Infrastructure;
using codeRR.SqlServer.Modules.Similarities.Entities;

namespace codeRR.SqlServer.Modules.Similarities.Queries
{
    [Component]
    public class GetSimilaritiesHandler : IQueryHandler<GetSimilarities, GetSimilaritiesResult>
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(GetSimilaritiesHandler));
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetSimilaritiesHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetSimilaritiesResult> ExecuteAsync(GetSimilarities query)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText =
                    @"select Name, Properties from IncidentContextCollections 
                            where IncidentId = @incidentId";
                cmd.AddParameter("incidentId", query.IncidentId);

                var collections = new List<GetSimilaritiesCollection>();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var json = (string) reader["Properties"];
                        var properties = OneTrueSerializer.Deserialize<ContextCollectionPropertyDbEntity[]>(json);
                        var col = new GetSimilaritiesCollection {Name = reader.GetString(0)};
                        col.Similarities = (from prop in properties
                            let values =
                                prop.Values.Select(x => new GetSimilaritiesValue(x.Value, x.Percentage, x.Count))
                            select new GetSimilaritiesSimilarity(prop.Name) {Values = values.ToArray()}
                            ).ToArray();
                        collections.Add(col);
                    }
                }

                return new GetSimilaritiesResult {Collections = collections.ToArray()};
            }
        }
    }
}