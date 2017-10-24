using System.Threading.Tasks;
using codeRR.Server.Api.Core.ApiKeys.Queries;
using codeRR.Server.SqlServer.Core.ApiKeys.Mappings;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.ApiKeys.Queries
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

        public async Task<ListApiKeysResult> HandleAsync(IMessageContext context, ListApiKeys query)
        {
            var keys =
                await
                    _unitOfWork.ToListAsync(_mapper,
                        "SELECT ID, GeneratedKey ApiKey, ApplicationName FROM ApiKeys ORDER BY ApplicationName");
            return new ListApiKeysResult {Keys = keys.ToArray()};
        }
    }
}