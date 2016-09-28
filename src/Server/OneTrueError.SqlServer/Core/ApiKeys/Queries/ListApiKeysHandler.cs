using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using OneTrueError.Api.Core.ApiKeys.Queries;
using OneTrueError.SqlServer.Core.ApiKeys.Mappings;

namespace OneTrueError.SqlServer.Core.ApiKeys.Queries
{
    [Component(RegisterAsSelf = true)]
    public class ListApiKeysHandler : IQueryHandler<ListApiKeys, ListApiKeysResult>
    {
        private IAdoNetUnitOfWork _unitOfWork;
        private MirrorMapper<ListApiKeysResultItem> _mapper = new MirrorMapper<ListApiKeysResultItem>();

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
