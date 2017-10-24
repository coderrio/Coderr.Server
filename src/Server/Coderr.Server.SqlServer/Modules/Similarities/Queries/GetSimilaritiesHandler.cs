using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Modules.ContextData.Queries;
using codeRR.Server.Infrastructure;
using codeRR.Server.SqlServer.Modules.Similarities.Entities;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using log4net;

namespace codeRR.Server.SqlServer.Modules.Similarities.Queries
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

        public async Task<GetSimilaritiesResult> HandleAsync(IMessageContext context, GetSimilarities query)
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
                        var properties = CoderrDtoSerializer.Deserialize<ContextCollectionPropertyDbEntity[]>(json);
                        var col = new GetSimilaritiesCollection {Name = reader.GetString(0)};
                        col.Similarities = (from prop in properties
                            let values =
                                prop.Values.Select(x => new GetSimilaritiesValue(x.Value, x.Percentage, x.Count))
                            select new GetSimilaritiesSimilarity(prop.Name) {Values = Enumerable.ToArray(values)}
                            ).ToArray();
                        collections.Add(col);
                    }
                }

                return new GetSimilaritiesResult {Collections = collections.ToArray()};
            }
        }
    }
}