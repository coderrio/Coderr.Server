using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using codeRR.Api.Core.ApiKeys.Queries;
using codeRR.SqlServer.Core.ApiKeys.Mappings;

namespace codeRR.SqlServer.Core.ApiKeys.Queries
{
    [Component(RegisterAsSelf = true)]
    public class ListApiKeysHandler : IQueryHandler<ListApiKeys, ListApiKeysResult>
    {
        private readonly MirrorMapper<ListApiKeysResultItem> _mapper = new MirrorMapper<ListApiKeysResultItem>();
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public ListApiKeysHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ListApiKeysResult> ExecuteAsync(ListApiKeys query)
        {
            var keys =
                await
                    _unitOfWork.ToListAsync(_mapper,
                        "SELECT ID, GeneratedKey ApiKey, ApplicationName FROM ApiKeys ORDER BY ApplicationName");
            return new ListApiKeysResult {Keys = keys.ToArray()};
        }
    }
}