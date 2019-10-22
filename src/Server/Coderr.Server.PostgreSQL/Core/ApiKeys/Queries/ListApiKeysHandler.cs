using System.Threading.Tasks;
using Coderr.Server.Api.Core.ApiKeys.Queries;
using Coderr.Server.PostgreSQL.Core.ApiKeys.Mappings;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.ApiKeys.Queries
{
    public class ListApiKeysHandler : IQueryHandler<ListApiKeys, ListApiKeysResult>
    {
        private readonly MirrorMapper<ListApiKeysResultItem> _mapper = new MirrorMapper<ListApiKeysResultItem>();
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ListApiKeysHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ListApiKeysResult> HandleAsync(IMessageContext context, ListApiKeys query)
        {
            var keys =
                await
                    _unitOfWork.ToListAsync(_mapper,
                        "SELECT ID, GeneratedKey ApiKey, ApplicationName FROM ApiKeys ORDER BY ApplicationName;");
            return new ListApiKeysResult {Keys = keys.ToArray()};
        }
    }
}