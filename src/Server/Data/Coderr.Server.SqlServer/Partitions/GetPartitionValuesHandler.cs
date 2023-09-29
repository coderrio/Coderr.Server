using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Partitions.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Partitions
{
    internal class GetPartitionValuesHandler : IQueryHandler<GetPartitionValues, GetPartitionValuesResult>
    {
        private readonly IAdoNetUnitOfWork _adoNetUnitOfWork;

        public GetPartitionValuesHandler(IAdoNetUnitOfWork adoNetUnitOfWork)
        {
            _adoNetUnitOfWork = adoNetUnitOfWork;
        }

        public async Task<GetPartitionValuesResult> HandleAsync(IMessageContext context, GetPartitionValues message)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                var sql = @"SELECT ipv.Id, ipv.PartitionId, IncidentId, ReceivedAtUtc, Value
FROM IncidentPartitionValues ipv
JOIN ApplicationPartitionValues apv ON (apv.Id = ipv.ValueId)
WHERE ipv.PartitionId = @partitionId";
                cmd.AddParameter("partitionId", message.PartitionId);
                if (message.IncidentId.HasValue)
                {
                    sql += " AND IncidentId = @incidentId";
                    cmd.AddParameter("incidentId", message.IncidentId);
                }

                sql += " ORDER BY ReceivedAtUtc DESC";

                if (message.PageNumber.HasValue)
                {
                    var offset = (message.PageNumber.Value - 1) * message.PageSize;
                    sql += $"OFFSET {offset} ROWS FETCH NEXT {message.PageSize} ROWS ONLY";
                }
                else
                    sql = sql.Insert(7, $"TOP({message.PageSize}) ");

                cmd.CommandText = sql;
                var items = await cmd.ToListAsync<GetPartitionValuesResultItem>();
                return new GetPartitionValuesResult {Items = items.ToArray()};
            }
        }
    }
}